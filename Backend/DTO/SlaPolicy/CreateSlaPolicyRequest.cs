namespace Backend.DTO.SlaPolicy;

public class CreateSlaPolicyRequest
{
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? PriorityId { get; set; }

    public int FirstResponseMinutes { get; set; }

    public int ResolutionMinutes { get; set; }

    public bool IsDefault { get; set; }
}
