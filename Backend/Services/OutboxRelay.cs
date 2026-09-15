using Backend.Domain;
using Backend.Services;
using NHibernate;

namespace Backend.Infrastructure;

public class OutboxRelay : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxRelay> _logger;

    private const int BatchSize = 50;
    private static readonly TimeSpan PollInterval =
        TimeSpan.FromSeconds(1);

    public OutboxRelay(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxRelay> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Outbox relay started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while processing outbox messages.");
            }

            try
            {
                await Task.Delay(
                    PollInterval,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Outbox relay stopped.");
    }

    private async Task ProcessBatchAsync(
        CancellationToken cancellationToken)
    {
        var messages =
            await ClaimMessagesAsync(
                cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        _logger.LogInformation(
            "Claimed {Count} outbox messages.",
            messages.Count);

        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await PublishMessageAsync(
                message,
                cancellationToken);
        }
    }

    private async Task<List<OutboxMessage>> ClaimMessagesAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var session =
            scope.ServiceProvider
                .GetRequiredService<NHibernate.ISession>();

        using var transaction =
            session.BeginTransaction();

        var outboxService =
            new OutboxService(session);

        var messages =
            await outboxService.ClaimMessagesAsync(
                BatchSize,
                cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);

        return messages;
    }

    private async Task PublishMessageAsync(
        OutboxMessage message,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope =
                _scopeFactory.CreateScope();

            var publisher =
                scope.ServiceProvider
                    .GetRequiredService<RabbitMqPublisher>();

            await publisher.PublishAsync(
                message.EventType,
                message.Payload,
                cancellationToken);

            await MarkAsPublishedAsync(
                message.OutboxMessageId,
                cancellationToken);

            _logger.LogInformation(
                "Published outbox message {OutboxMessageId} with event type {EventType}.",
                message.OutboxMessageId,
                message.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish outbox message {OutboxMessageId}.",
                message.OutboxMessageId);

            await MarkAsFailedAsync(
                message.OutboxMessageId,
                ex.Message,
                cancellationToken);
        }
    }

    private async Task MarkAsPublishedAsync(
        Guid outboxMessageId,
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var session =
            scope.ServiceProvider
                .GetRequiredService<NHibernate.ISession>();

        using var transaction =
            session.BeginTransaction();

        var message =
            await session.GetAsync<OutboxMessage>(
                outboxMessageId,
                cancellationToken);

        if (message is null)
        {
            await transaction.CommitAsync(
                cancellationToken);

            return;
        }

        message.PublishedAt =
            DateTime.UtcNow;

        message.LockedAt = null;

        await session.FlushAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);
    }

    private async Task MarkAsFailedAsync(
        Guid outboxMessageId,
        string error,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope =
                _scopeFactory.CreateScope();

            var session =
                scope.ServiceProvider
                    .GetRequiredService<NHibernate.ISession>();

            using var transaction =
                session.BeginTransaction();

            var message =
                await session.GetAsync<OutboxMessage>(
                    outboxMessageId,
                    cancellationToken);

            if (message is null)
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return;
            }

            message.RetryCount++;

            message.LastError =
                error.Length > 4000
                    ? error[..4000]
                    : error;

            message.LockedAt = null;

            await session.FlushAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to update failed state for outbox message {OutboxMessageId}.",
                outboxMessageId);
        }
    }
}