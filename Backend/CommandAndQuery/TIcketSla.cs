using Backend.DTO.TicketSla;
using MediatR;

namespace Backend.CommandAndQuery;

public record GetTicketSlaQuery(
    Guid TicketId,
    Guid OrganizationId
) : IRequest<TicketSlaResponse>;
