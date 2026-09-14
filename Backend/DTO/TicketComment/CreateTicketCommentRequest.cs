namespace Backend.DTO.TicketComment;

public class CreateTicketCommentRequest
{
    public Guid OrganizationId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string CommentText { get; set; } = string.Empty;

    public bool IsInternal { get; set; }
}
