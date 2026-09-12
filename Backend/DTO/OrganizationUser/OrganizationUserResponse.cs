namespace Backend.DTO.OrganizationUser;

public class OrganizationUserResponse
{
    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
