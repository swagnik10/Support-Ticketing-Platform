namespace Backend.DTO.Ticket;

public class TicketResponse
{
    public Guid TicketId { get; set; }

    public Guid OrganizationId { get; set; }

    public long TicketNumber { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid CustomerUserId { get; set; }

    public Guid? AssignedAgentId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid PriorityId { get; set; }

    public Guid StatusId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
