using Backend.DTO.TicketComment;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateTicketCommentCommand(
    Guid TicketId,
    CreateTicketCommentRequest Request
) : IRequest<TicketCommentResponse>;

public record GetTicketCommentsQuery(
    Guid TicketId,
    Guid OrganizationId
) : IRequest<IList<TicketCommentResponse>>;

public record GetTicketCommentQuery(
    Guid TicketId,
    Guid CommentId,
    Guid OrganizationId
) : IRequest<TicketCommentResponse>;

public record UpdateTicketCommentCommand(
    Guid TicketId,
    Guid CommentId,
    UpdateTicketCommentRequest Request
) : IRequest<TicketCommentResponse>;

public record DeleteTicketCommentCommand(
    Guid TicketId,
    Guid CommentId,
    Guid OrganizationId,
    Guid AuthorUserId
) : IRequest<bool>;