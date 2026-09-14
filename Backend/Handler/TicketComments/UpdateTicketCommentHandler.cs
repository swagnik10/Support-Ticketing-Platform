using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketComment;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketComments;

public class UpdateTicketCommentHandler
    : IRequestHandler<UpdateTicketCommentCommand, TicketCommentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateTicketCommentHandler> _logger;

    public UpdateTicketCommentHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateTicketCommentHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketCommentResponse> Handle(
        UpdateTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var comment = await unitOfWork.Session.Query<TicketComment>()
                .FirstOrDefaultAsync(
                    x =>
                        x.CommentId == request.CommentId &&
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (comment == null)
                throw new KeyNotFoundException("Comment was not found.");

            var authorMembership =
                await unitOfWork.Session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == request.Request.OrganizationId &&
                            x.UserId == request.Request.AuthorUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (authorMembership == null)
                throw new UnauthorizedAccessException(
                    "Comment author is not an active member of the organization.");

            var commentText = request.Request.CommentText?.Trim();

            if (string.IsNullOrWhiteSpace(commentText))
                throw new ArgumentException("Comment text is required.");

            var oldValue = comment.CommentText;
            var now = DateTime.UtcNow;

            comment.CommentText = commentText;
            comment.IsInternal = request.Request.IsInternal;
            comment.UpdatedAt = now;

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = comment.OrganizationId,
                TicketId = comment.TicketId,
                EventType = "ticket.comment_updated",
                ActorUserId = request.Request.AuthorUserId,
                OldValue = oldValue,
                NewValue = comment.CommentText,
                Metadata = null,
                CorrelationId = null,
                OccurredAt = now
            };

            await unitOfWork.Session.SaveAsync(
                ticketEvent,
                cancellationToken);

            await unitOfWork.Session.FlushAsync(cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Updated comment {CommentId} for ticket {TicketId}",
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
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}