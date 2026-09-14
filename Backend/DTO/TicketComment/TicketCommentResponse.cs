namespace Backend.DTO.TicketComment;

public class TicketCommentResponse
{
    public Guid CommentId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid TicketId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string CommentText { get; set; } = string.Empty;

    public bool IsInternal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
