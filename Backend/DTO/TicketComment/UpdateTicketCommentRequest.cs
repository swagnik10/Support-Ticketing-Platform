namespace Backend.DTO.TicketComment;

public class UpdateTicketCommentRequest
{
    public Guid OrganizationId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string CommentText { get; set; } = string.Empty;

    public bool IsInternal { get; set; }
}