using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketComments;

public class DeleteTicketCommentHandler
    : IRequestHandler<DeleteTicketCommentCommand, bool>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<DeleteTicketCommentHandler> _logger;

    public DeleteTicketCommentHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<DeleteTicketCommentHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeleteTicketCommentCommand request,
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
                        x.OrganizationId == request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (comment == null)
                throw new KeyNotFoundException("Comment was not found.");

            var authorMembership =
                await unitOfWork.Session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == request.OrganizationId &&
                            x.UserId == request.AuthorUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (authorMembership == null)
                throw new UnauthorizedAccessException(
                    "Comment author is not an active member of the organization.");

            var now = DateTime.UtcNow;

            comment.DeletedAt = now;
            comment.UpdatedAt = now;

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = comment.OrganizationId,
                TicketId = comment.TicketId,
                EventType = "ticket.comment_deleted",
                ActorUserId = request.AuthorUserId,
                OldValue = comment.CommentText,
                NewValue = null,
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
                "Deleted comment {CommentId} from ticket {TicketId}",
                comment.CommentId,
                comment.TicketId);

            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}