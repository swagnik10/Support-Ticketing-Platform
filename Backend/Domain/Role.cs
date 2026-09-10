namespace Backend.Domain;

public class Role
{
    public virtual Guid RoleId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string? Description { get; set; }

    public virtual bool IsSystemRole { get; set; }

    public virtual DateTime CreatedAt { get; set; }
}