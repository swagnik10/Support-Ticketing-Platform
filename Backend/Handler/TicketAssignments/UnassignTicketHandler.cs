using System.Text.Json;
using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketAssignments;

public class UnassignTicketHandler
    : IRequestHandler<UnassignTicketCommand, bool>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UnassignTicketHandler> _logger;

    public UnassignTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UnassignTicketHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<bool> Handle(
        UnassignTicketCommand request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var ticket = await unitOfWork.Session
                .Query<Ticket>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket was not found.");

            var actorMembership = await unitOfWork.Session
                .Query<OrganizationUser>()
                .FirstOrDefaultAsync(
                    x =>
                        x.OrganizationId == request.OrganizationId &&
                        x.UserId == request.AssignedByUserId &&
                        x.IsActive &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (actorMembership == null)
                throw new UnauthorizedAccessException(
                    "The user is not an active member of the organization.");

            var currentAssignment = await unitOfWork.Session
                .Query<TicketAssignment>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.OrganizationId &&
                        x.UnassignedAt == null,
                    cancellationToken);

            if (currentAssignment == null)
                throw new KeyNotFoundException(
                    "The ticket is not currently assigned.");

            var now = DateTime.UtcNow;

            var previousAgentId =
                currentAssignment.AssignedToUserId;

            currentAssignment.UnassignedAt = now;

            ticket.AssignedAgentId = null;
            ticket.UpdatedAt = now;

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                TicketId = request.TicketId,
                EventType = "ticket.unassigned",
                ActorUserId = request.AssignedByUserId,
                OldValue = JsonSerializer.Serialize(
                    new
                    {
                        assignedAgentId = previousAgentId
                    }),
                NewValue = JsonSerializer.Serialize(
                    new
                    {
                        assignedAgentId = (Guid?)null
                    }),
                Metadata = JsonSerializer.Serialize(
                    new
                    {
                        assignmentId =
                            currentAssignment.AssignmentId
                    }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = now
            };

            await unitOfWork.Session.SaveAsync(
                ticketEvent,
                cancellationToken);

            await unitOfWork.Session.FlushAsync(
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} unassigned from user {AssignedToUserId} for organization {OrganizationId}",
                request.TicketId,
                previousAgentId,
                request.OrganizationId);

            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}