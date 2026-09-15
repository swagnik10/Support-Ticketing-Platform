using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketAssignment;
using Backend.Services;
using MediatR;
using NHibernate.Linq;
using System.Text.Json;

namespace Backend.Handler.TicketAssignments;

public class ReassignTicketHandler
    : IRequestHandler<ReassignTicketCommand, TicketAssignmentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<ReassignTicketHandler> _logger;
    private readonly OutboxService _outboxService;

    public ReassignTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<ReassignTicketHandler> logger,
        OutboxService outboxService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
        _outboxService = outboxService;
    }

    public async Task<TicketAssignmentResponse> Handle(
        ReassignTicketCommand request,
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

            var currentAssignment = await unitOfWork.Session
                .Query<TicketAssignment>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.UnassignedAt == null,
                    cancellationToken);

            if (currentAssignment == null)
                throw new KeyNotFoundException(
                    "The ticket does not currently have an assignment.");

            if (currentAssignment.AssignedToUserId ==
                request.Request.AssignedToUserId)
            {
                throw new InvalidOperationException(
                    "The ticket is already assigned to this agent.");
            }

            var now = DateTime.UtcNow;

            var previousAgentId = currentAssignment.AssignedToUserId;

            currentAssignment.UnassignedAt = now;

            var newAssignment = new TicketAssignment
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

            ticket.AssignedAgentId =
                request.Request.AssignedToUserId;

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
                        assignedAgentId = previousAgentId
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
                        assignmentId = newAssignment.AssignmentId,
                        previousAssignmentId =
                            currentAssignment.AssignmentId
                    }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = now
            };

            await unitOfWork.Session.SaveAsync(
                newAssignment,
                cancellationToken);

            await unitOfWork.Session.SaveAsync(
                ticketEvent,
                cancellationToken);

            await _outboxService.AddAsync(ticketEvent, cancellationToken);

            await unitOfWork.Session.FlushAsync(
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} reassigned from user {PreviousAgentId} to user {AssignedToUserId} for organization {OrganizationId}",
                request.TicketId,
                previousAgentId,
                request.Request.AssignedToUserId,
                request.Request.OrganizationId);

            return new TicketAssignmentResponse
            {
                AssignmentId = newAssignment.AssignmentId,
                OrganizationId = newAssignment.OrganizationId,
                TicketId = newAssignment.TicketId,
                AssignedToUserId = newAssignment.AssignedToUserId,
                AssignedByUserId = newAssignment.AssignedByUserId,
                AssignedAt = newAssignment.AssignedAt,
                UnassignedAt = newAssignment.UnassignedAt,
                CreatedAt = newAssignment.CreatedAt
            };
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}