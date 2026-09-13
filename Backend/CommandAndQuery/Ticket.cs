using Backend.DTO.Ticket;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateTicketCommand(
    CreateTicketRequest Request
) : IRequest<TicketResponse>;

public record GetTicketsQuery(
    Guid OrganizationId
) : IRequest<IList<TicketResponse>>;

public record GetTicketQuery(
    Guid OrganizationId,
    Guid TicketId
) : IRequest<TicketResponse>;

public record UpdateTicketCommand(
    Guid TicketId,
    UpdateTicketRequest Request
) : IRequest<TicketResponse>;

public record UpdateTicketStatusCommand(
    Guid TicketId,
    UpdateTicketStatusRequest Request
) : IRequest<TicketResponse>;
