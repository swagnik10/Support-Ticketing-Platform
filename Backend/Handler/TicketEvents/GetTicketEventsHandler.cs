using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketEvent;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketEvents;

public class GetTicketEventsHandler
    : IRequestHandler<GetTicketEventsQuery, IList<TicketEventResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketEventsHandler> _logger;

    public GetTicketEventsHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketEventsHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketEventResponse>> Handle(
        GetTicketEventsQuery request,
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

        var events = await unitOfWork.Session
            .Query<TicketEvent>()
            .Where(
                x =>
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId)
            .OrderBy(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {EventCount} events for ticket {TicketId}",
            events.Count,
            request.TicketId);

        return events
            .Select(MapToResponse)
            .ToList();
    }

    private static TicketEventResponse MapToResponse(
        TicketEvent ticketEvent)
    {
        return new TicketEventResponse
        {
            EventId = ticketEvent.EventId,
            OrganizationId = ticketEvent.OrganizationId,
            TicketId = ticketEvent.TicketId,
            EventType = ticketEvent.EventType,
            ActorUserId = ticketEvent.ActorUserId,
            OldValue = ticketEvent.OldValue,
            NewValue = ticketEvent.NewValue,
            Metadata = ticketEvent.Metadata,
            CorrelationId = ticketEvent.CorrelationId,
            OccurredAt = ticketEvent.OccurredAt
        };
    }
}