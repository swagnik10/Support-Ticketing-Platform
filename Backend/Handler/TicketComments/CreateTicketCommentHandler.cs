using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketComment;
using Backend.Services;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketComments;

public class CreateTicketCommentHandler
    : IRequestHandler<CreateTicketCommentCommand, TicketCommentResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateTicketCommentHandler> _logger;
    private readonly OutboxService _outboxService;


    public CreateTicketCommentHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateTicketCommentHandler> logger,
        OutboxService outboxService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
        _outboxService = outboxService;
    }

    public async Task<TicketCommentResponse> Handle(
        CreateTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var ticket = await unitOfWork.Session.Query<Ticket>()
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == request.TicketId &&
                        x.OrganizationId == request.Request.OrganizationId &&
                        x.DeletedAt == null,
                    cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket was not found.");

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

            var now = DateTime.UtcNow;

            var comment = new TicketComment
            {
                CommentId = Guid.NewGuid(),
                OrganizationId = request.Request.OrganizationId,
                TicketId = request.TicketId,
                AuthorUserId = request.Request.AuthorUserId,
                CommentText = commentText,
                IsInternal = request.Request.IsInternal,
                CreatedAt = now,
                UpdatedAt = now
            };

            await unitOfWork.Session.SaveAsync(
                comment,
                cancellationToken);

            var correlationId = Guid.NewGuid();

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = comment.OrganizationId,
                TicketId = comment.TicketId,
                EventType = "ticket.comment_added",
                ActorUserId = comment.AuthorUserId,
                OldValue = null,
                NewValue = comment.CommentText,
                Metadata = null,
                CorrelationId = correlationId,
                OccurredAt = now
            };

            await unitOfWork.Session.SaveAsync(
                ticketEvent,
                cancellationToken);

            ticket.UpdatedAt = now;

            await _outboxService.AddAsync(ticketEvent, cancellationToken);

            await unitOfWork.Session.FlushAsync(cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Created comment {CommentId} for ticket {TicketId}",
                comment.CommentId,
                comment.TicketId);

            return MapToResponse(comment);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
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