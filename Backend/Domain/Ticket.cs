namespace Backend.Domain;

public class Ticket
{
    public virtual Guid TicketId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual long TicketNumber { get; set; }

    public virtual string Subject { get; set; } = string.Empty;

    public virtual string Description { get; set; } = string.Empty;

    public virtual Guid CustomerUserId { get; set; }

    public virtual Guid? AssignedAgentId { get; set; }

    public virtual Guid CategoryId { get; set; }

    public virtual Guid PriorityId { get; set; }

    public virtual Guid StatusId { get; set; }

    public virtual Guid CreatedByUserId { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }

    public virtual DateTime? ResolvedAt { get; set; }

    public virtual DateTime? ClosedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }
}