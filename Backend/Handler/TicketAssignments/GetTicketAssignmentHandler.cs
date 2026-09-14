using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketAssignment;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketAssignments;

public class GetTicketAssignmentHandler
    : IRequestHandler<GetTicketAssignmentQuery, TicketAssignmentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketAssignmentHandler> _logger;

    public GetTicketAssignmentHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketAssignmentHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketAssignmentResponse> Handle(
        GetTicketAssignmentQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var ticketExists = await unitOfWork.Session
            .Query<Ticket>()
            .AnyAsync(
                x =>
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId &&
                    x.DeletedAt == null,
                cancellationToken);

        if (!ticketExists)
            throw new KeyNotFoundException("Ticket was not found.");

        var assignment = await unitOfWork.Session
            .Query<TicketAssignment>()
            .FirstOrDefaultAsync(
                x =>
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId &&
                    x.UnassignedAt == null,
                cancellationToken);

        if (assignment == null)
            throw new KeyNotFoundException(
                "The ticket is not currently assigned.");

        _logger.LogInformation(
            "Retrieved assignment for ticket {TicketId} for organization {OrganizationId}",
            request.TicketId,
            request.OrganizationId);

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
}