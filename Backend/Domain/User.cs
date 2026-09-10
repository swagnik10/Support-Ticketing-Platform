namespace Backend.Domain;

public class User
{
    public virtual Guid UserId { get; set; }

    public virtual string Email { get; set; } = string.Empty;

    public virtual string? PasswordHash { get; set; }

    public virtual string FirstName { get; set; } = string.Empty;

    public virtual string? LastName { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }
}