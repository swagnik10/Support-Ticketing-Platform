namespace Backend.Domain;

public class TicketCategory
{
    public virtual Guid CategoryId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string? Description { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }
}