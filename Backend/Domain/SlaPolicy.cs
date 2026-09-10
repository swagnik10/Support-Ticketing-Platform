namespace Backend.Domain;

public class SlaPolicy
{
    public virtual Guid SlaPolicyId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string? Description { get; set; }

    public virtual Guid? PriorityId { get; set; }

    public virtual int FirstResponseMinutes { get; set; }

    public virtual int ResolutionMinutes { get; set; }

    public virtual bool IsDefault { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }
}