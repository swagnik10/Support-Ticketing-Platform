using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Ticket;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Tickets;

public class GetTicketHandler
    : IRequestHandler<GetTicketQuery, TicketResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketHandler> _logger;

    public GetTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketResponse> Handle(
        GetTicketQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var ticket =
            await unitOfWork.Session.Query<Ticket>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (ticket == null)
            throw new KeyNotFoundException("Ticket was not found.");

        _logger.LogInformation(
            "Retrieved ticket {TicketId} for organization {OrganizationId}",
            ticket.TicketId,
            request.OrganizationId);

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