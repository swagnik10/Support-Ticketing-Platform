using System.Text.Json;
using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketAssignment;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketAssignments;

public class AssignTicketHandler
    : IRequestHandler<AssignTicketCommand, TicketAssignmentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<AssignTicketHandler> _logger;

    public AssignTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<AssignTicketHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketAssignmentResponse> Handle(
        AssignTicketCommand request,
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
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket was not found.");

            var assignerMembership = await unitOfWork.Session
                .Query<OrganizationUser>()
                .FirstOrDefaultAsync(
                    x =>
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.UserId == request.Request.AssignedByUserId &&
                        x.IsActive &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (assignerMembership == null)
                throw new UnauthorizedAccessException(
                    "The assigning user is not an active member of the organization.");

            var agentMembership = await unitOfWork.Session
                .Query<OrganizationUser>()
                .FirstOrDefaultAsync(
                    x =>
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.UserId == request.Request.AssignedToUserId &&
                        x.IsActive &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (agentMembership == null)
                throw new KeyNotFoundException(
                    "The assigned user is not an active member of the organization.");

            var agentRole = await unitOfWork.Session
                .Query<Role>()
                .FirstOrDefaultAsync(
                    x =>
                        x.RoleId == agentMembership.RoleId &&
                        x.Name == "Agent",
                    cancellationToken);

            if (agentRole == null)
                throw new InvalidOperationException(
                    "The assigned user does not have the Agent role.");

            var existingAssignment = await unitOfWork.Session
                .Query<TicketAssignment>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.UnassignedAt == null,
                    cancellationToken);

            if (existingAssignment != null)
                throw new InvalidOperationException(
                    "The ticket is already assigned. Use the reassign operation to change the assigned agent.");

            var now = DateTime.UtcNow;

            var assignment = new TicketAssignment
            {
                AssignmentId = Guid.NewGuid(),
                OrganizationId = request.Request.OrganizationId,
                TicketId = request.TicketId,
                AssignedToUserId = request.Request.AssignedToUserId,
                AssignedByUserId = request.Request.AssignedByUserId,
                AssignedAt = now,
                UnassignedAt = null,
                CreatedAt = now
            };

            ticket.AssignedAgentId = request.Request.AssignedToUserId;
            ticket.UpdatedAt = now;

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = request.Request.OrganizationId,
                TicketId = request.TicketId,
                EventType = "ticket.assigned",
                ActorUserId = request.Request.AssignedByUserId,
                OldValue = JsonSerializer.Serialize(
                    new
                    {
                        assignedAgentId = (Guid?)null
                    }),
                NewValue = JsonSerializer.Serialize(
                    new
                    {
                        assignedAgentId =
                            request.Request.AssignedToUserId
                    }),
                Metadata = JsonSerializer.Serialize(
                    new
                    {
                        assignmentId = assignment.AssignmentId
                    }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = now
            };

            await unitOfWork.Session.SaveAsync(
                assignment,
                cancellationToken);

            await unitOfWork.Session.SaveAsync(
                ticketEvent,
                cancellationToken);

            await unitOfWork.Session.FlushAsync(
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} assigned to user {AssignedToUserId} for organization {OrganizationId}",
                request.TicketId,
                request.Request.AssignedToUserId,
                request.Request.OrganizationId);

            return new TicketAssignmentResponse
            {
                AssignmentId = assignment.AssignmentId,
                OrganizationId = assignment.OrganizationId,
                TicketId = assignment.TicketId,
                AssignedToUserId = assignment.AssignedToUserId,
                AssignedByUserId = assignment.AssignedByUserId,
                AssignedAt = assignment.AssignedAt,
                UnassignedAt = assignment.UnassignedAt,
                CreatedAt = assignment.CreatedAt
            };
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}