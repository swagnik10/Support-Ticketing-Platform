using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketComment;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketComments;

public class GetTicketCommentsHandler
    : IRequestHandler<GetTicketCommentsQuery, IList<TicketCommentResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketCommentsHandler> _logger;

    public GetTicketCommentsHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketCommentsHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketCommentResponse>> Handle(
        GetTicketCommentsQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var ticketExists = await unitOfWork.Session.Query<Ticket>()
            .AnyAsync(
                x =>
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId &&
                    x.DeletedAt == null,
                cancellationToken);

        if (!ticketExists)
            throw new KeyNotFoundException("Ticket was not found.");

        var comments = await unitOfWork.Session.Query<TicketComment>()
            .Where(
                x =>
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId &&
                    x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {CommentCount} comments for ticket {TicketId}",
            comments.Count,
            request.TicketId);

        return comments
            .Select(MapToResponse)
            .ToList();
    }

    private static TicketCommentResponse MapToResponse(
        TicketComment comment)
    {
        return new TicketCommentResponse
        {
            CommentId = comment.CommentId,
            OrganizationId = comment.OrganizationId,
            TicketId = comment.TicketId,
            AuthorUserId = comment.AuthorUserId,
            CommentText = comment.CommentText,
            IsInternal = comment.IsInternal,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}