using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Ticket;
using Backend.Services;
using MediatR;
using NHibernate.Linq;
using System.Text.Json;

namespace Backend.Handler.Tickets;

public class UpdateTicketStatusHandler
    : IRequestHandler<UpdateTicketStatusCommand, TicketResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateTicketStatusHandler> _logger;
    private readonly OutboxService _outboxService;

    public UpdateTicketStatusHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateTicketStatusHandler> logger,
        OutboxService outboxService)
    {
        _unitOfWorkFactory = unitOfWorkFactory; 
        _logger = logger;
        _outboxService = outboxService;
    }

    public async Task<TicketResponse> Handle(
        UpdateTicketStatusCommand request,
        CancellationToken cancellationToken)
    {
        var data = request.Request;

        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var session = unitOfWork.Session;

            var ticket =
                await session.Query<Ticket>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.TicketId == request.TicketId &&
                            x.OrganizationId == data.OrganizationId &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket was not found.");

            var actorMembership =
                await session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == data.OrganizationId &&
                            x.UserId == data.ActorUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (actorMembership == null)
                throw new InvalidOperationException(
                    "Actor user does not belong to the organization.");

            var newStatus =
                await session.Query<TicketStatus>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.StatusId == data.StatusId &&
                            x.OrganizationId == data.OrganizationId &&
                            x.IsActive,
                        cancellationToken);

            if (newStatus == null)
                throw new InvalidOperationException(
                    "Ticket status was not found or is inactive.");

            if (ticket.StatusId == newStatus.StatusId)
                throw new InvalidOperationException(
                    "Ticket is already in the requested status.");

            var oldStatus =
                await session.Query<TicketStatus>()
                    .FirstOrDefaultAsync(
                        x => x.StatusId == ticket.StatusId,
                        cancellationToken);

            var now = DateTime.UtcNow;

            var oldValue = JsonSerializer.Serialize(new
            {
                statusId = ticket.StatusId,
                statusCode = oldStatus?.StatusCode,
                statusName = oldStatus?.Name
            });

            ticket.StatusId = newStatus.StatusId;
            ticket.UpdatedAt = now;

            /*
             * No transition matrix is enforced yet.
             *
             * RESOLVED:
             *     ResolvedAt = now
             *     ClosedAt = null
             *
             * CLOSED:
             *     ResolvedAt = now if not already resolved
             *     ClosedAt = now
             *
             * Other statuses:
             *     lifecycle timestamps are cleared.
             */
            if (string.Equals(
                    newStatus.StatusCode,
                    "RESOLVED",
                    StringComparison.OrdinalIgnoreCase))
            {
                ticket.ResolvedAt ??= now;
                ticket.ClosedAt = null;
            }
            else if (string.Equals(
                         newStatus.StatusCode,
                         "CLOSED",
                         StringComparison.OrdinalIgnoreCase))
            {
                ticket.ResolvedAt ??= now;
                ticket.ClosedAt ??= now;
            }
            else
            {
                ticket.ResolvedAt = null;
                ticket.ClosedAt = null;
            }

            var newValue = JsonSerializer.Serialize(new
            {
                statusId = newStatus.StatusId,
                statusCode = newStatus.StatusCode,
                statusName = newStatus.Name
            });

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = ticket.OrganizationId,
                TicketId = ticket.TicketId,
                EventType = "ticket.status_changed",
                ActorUserId = data.ActorUserId,
                OldValue = oldValue,
                NewValue = newValue,
                Metadata = JsonSerializer.Serialize(new
                {
                    source = "api"
                }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = now
            };

            await session.SaveAsync(ticketEvent, cancellationToken);

            await _outboxService.AddAsync(ticketEvent, cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} status changed from {OldStatus} to {NewStatus} by user {ActorUserId}",
                ticket.TicketId,
                oldStatus?.StatusCode,
                newStatus.StatusCode,
                data.ActorUserId);

            return MapToResponse(ticket);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static TicketResponse MapToResponse(Ticket ticket)
    {
        return new TicketResponse
        {
            TicketId = ticket.TicketId,
            OrganizationId = ticket.OrganizationId,
            TicketNumber = ticket.TicketNumber,
            Subject = ticket.Subject,
            Description = ticket.Description,
            CustomerUserId = ticket.CustomerUserId,
            AssignedAgentId = ticket.AssignedAgentId,
            CategoryId = ticket.CategoryId,
            PriorityId = ticket.PriorityId,
            StatusId = ticket.StatusId,
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt
        };
    }
}