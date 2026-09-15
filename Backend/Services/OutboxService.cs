using System.Text.Json;
using Backend.Domain;


namespace Backend.Services;

public class OutboxService
{
    private readonly NHibernate.ISession _session;

    public OutboxService(NHibernate.ISession session)
    {
        _session = session;
    }

    public async Task AddAsync(
        TicketEvent ticketEvent,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(new
        {
            eventId = ticketEvent.EventId,
            organizationId = ticketEvent.OrganizationId,
            ticketId = ticketEvent.TicketId,
            eventType = ticketEvent.EventType,
            actorUserId = ticketEvent.ActorUserId,
            oldValue = ticketEvent.OldValue,
            newValue = ticketEvent.NewValue,
            metadata = ticketEvent.Metadata,
            correlationId = ticketEvent.CorrelationId,
            occurredAt = ticketEvent.OccurredAt
        });

        var outboxMessage = new OutboxMessage
        {
            OutboxMessageId = Guid.NewGuid(),
            OrganizationId = ticketEvent.OrganizationId,
            AggregateType = "Ticket",
            AggregateId = ticketEvent.TicketId,
            EventType = ticketEvent.EventType,
            Payload = payload,
            CorrelationId = ticketEvent.CorrelationId,
            OccurredAt = ticketEvent.OccurredAt,
            PublishedAt = null,
            RetryCount = 0,
            LastError = null,
            LockedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        await _session.SaveAsync(outboxMessage, cancellationToken);
    }
}