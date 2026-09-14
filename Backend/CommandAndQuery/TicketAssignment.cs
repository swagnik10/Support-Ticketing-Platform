using Backend.DTO.TicketAssignment;
using MediatR;

namespace Backend.CommandAndQuery;

public record AssignTicketCommand(
    Guid TicketId,
    AssignTicketRequest Request
) : IRequest<TicketAssignmentResponse>;

public record ReassignTicketCommand(
    Guid TicketId,
    AssignTicketRequest Request
) : IRequest<TicketAssignmentResponse>;

public record UnassignTicketCommand(
    Guid TicketId,
    Guid OrganizationId,
    Guid AssignedByUserId
) : IRequest<bool>;

public record GetTicketAssignmentQuery(
    Guid TicketId,
    Guid OrganizationId
) : IRequest<TicketAssignmentResponse>;
