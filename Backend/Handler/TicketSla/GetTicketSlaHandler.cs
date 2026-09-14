using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketSla;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketSla;

public class GetTicketSlaHandler
    : IRequestHandler<GetTicketSlaQuery, TicketSlaResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketSlaHandler> _logger;

    public GetTicketSlaHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketSlaHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketSlaResponse> Handle(
        GetTicketSlaQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var ticketExists =
            await unitOfWork.Session.Query<Ticket>()
                .AnyAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (!ticketExists)
            throw new KeyNotFoundException(
                "Ticket was not found.");

        var ticketSla =
            await unitOfWork.Session.Query<Backend.Domain.TicketSla>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.OrganizationId,
                    cancellationToken);

        if (ticketSla == null)
            throw new KeyNotFoundException(
                "SLA information was not found for this ticket.");

        _logger.LogInformation(
            "Retrieved SLA for ticket {TicketId}",
            request.TicketId);

        return new TicketSlaResponse
        {
            TicketSlaId = ticketSla.TicketSlaId,
            OrganizationId = ticketSla.OrganizationId,
            TicketId = ticketSla.TicketId,
            SlaPolicyId = ticketSla.SlaPolicyId,
            FirstResponseDueAt = ticketSla.FirstResponseDueAt,
            FirstRespondedAt = ticketSla.FirstRespondedAt,
            ResolutionDueAt = ticketSla.ResolutionDueAt,
            ResolvedAt = ticketSla.ResolvedAt,
            FirstResponseBreached = ticketSla.FirstResponseBreached,
            ResolutionBreached = ticketSla.ResolutionBreached,
            CreatedAt = ticketSla.CreatedAt,
            UpdatedAt = ticketSla.UpdatedAt
        };
    }
}