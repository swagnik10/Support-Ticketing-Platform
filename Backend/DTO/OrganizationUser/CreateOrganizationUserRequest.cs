namespace Backend.DTO.OrganizationUser;

public class CreateOrganizationUserRequest
{
    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }
}
