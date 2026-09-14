using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketComment;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketComments;

public class GetTicketCommentHandler
    : IRequestHandler<GetTicketCommentQuery, TicketCommentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketCommentHandler> _logger;

    public GetTicketCommentHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketCommentHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketCommentResponse> Handle(
        GetTicketCommentQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var comment = await unitOfWork.Session.Query<TicketComment>()
            .FirstOrDefaultAsync(
                x =>
                    x.CommentId == request.CommentId &&
                    x.TicketId == request.TicketId &&
                    x.OrganizationId == request.OrganizationId &&
                    x.DeletedAt == null,
                cancellationToken);

        if (comment == null)
            throw new KeyNotFoundException("Comment was not found.");

        _logger.LogInformation(
            "Retrieved comment {CommentId} for ticket {TicketId}",
            comment.CommentId,
            comment.TicketId);

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