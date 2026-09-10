namespace Backend.Domain;

public class TicketComment
{
    public virtual Guid CommentId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual Guid TicketId { get; set; }

    public virtual Guid AuthorUserId { get; set; }

    public virtual string CommentText { get; set; } = string.Empty;

    public virtual bool IsInternal { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }
}