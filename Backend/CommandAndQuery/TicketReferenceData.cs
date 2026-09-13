using Backend.DTO.TicketReference;
using MediatR;

namespace Backend.CommandAndQuery;

// Ticket Category
public record GetTicketCategoriesQuery(
    Guid OrganizationId
) : IRequest<IList<TicketCategoryResponse>>;

public record GetTicketCategoryQuery(
    Guid OrganizationId,
    Guid CategoryId
) : IRequest<TicketCategoryResponse>;

// Ticket Priority
public record GetTicketPrioritiesQuery(
    Guid OrganizationId
) : IRequest<IList<TicketPriorityResponse>>;

public record GetTicketPriorityQuery(
    Guid OrganizationId,
    Guid PriorityId
) : IRequest<TicketPriorityResponse>;

// Ticket Status
public record GetTicketStatusesQuery(
    Guid OrganizationId
) : IRequest<IList<TicketStatusResponse>>;

public record GetTicketStatusQuery(
    Guid OrganizationId,
    Guid StatusId
) : IRequest<TicketStatusResponse>;