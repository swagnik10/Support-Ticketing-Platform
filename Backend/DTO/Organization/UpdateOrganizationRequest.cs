namespace Backend.DTO.Organization;

public class UpdateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;
}
