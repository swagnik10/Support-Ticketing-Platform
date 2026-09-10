namespace Backend.Domain;

public class OrganizationUser
{
    public virtual Guid OrganizationId { get; set; }

    public virtual Guid UserId { get; set; }

    public virtual Guid RoleId { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime JoinedAt { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }
}