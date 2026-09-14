using Backend.DTO.TicketEvent;
using MediatR;

namespace Backend.CommandAndQuery;

public record GetTicketEventsQuery(
    Guid TicketId,
    Guid OrganizationId
) : IRequest<IList<TicketEventResponse>>;
