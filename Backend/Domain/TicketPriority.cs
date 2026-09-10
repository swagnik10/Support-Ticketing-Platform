namespace Backend.Domain;

public class TicketPriority
{
    public virtual Guid PriorityId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual int PriorityLevel { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }
}