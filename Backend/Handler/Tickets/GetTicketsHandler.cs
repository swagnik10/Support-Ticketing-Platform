using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Ticket;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Tickets;

public class GetTicketsHandler
    : IRequestHandler<GetTicketsQuery, IList<TicketResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketsHandler> _logger;

    public GetTicketsHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketsHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketResponse>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var tickets =
            await unitOfWork.Session.Query<Ticket>()
                .Where(x =>
                    x.OrganizationId == request.OrganizationId &&
                    x.DeletedAt == null)
                .OrderByDescending(x => x.TicketNumber)
                .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {TicketCount} tickets for organization {OrganizationId}",
            tickets.Count,
            request.OrganizationId);

        return tickets
            .Select(MapToResponse)
            .ToList();
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