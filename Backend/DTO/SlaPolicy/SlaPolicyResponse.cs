namespace Backend.DTO.SlaPolicy;

public class SlaPolicyResponse
{
    public Guid SlaPolicyId { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? PriorityId { get; set; }

    public int FirstResponseMinutes { get; set; }

    public int ResolutionMinutes { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
