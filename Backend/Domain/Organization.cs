namespace Backend.Domain;

public class Organization
{
    public virtual Guid OrganizationId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string Slug { get; set; } = string.Empty;

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }
}
