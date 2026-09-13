namespace Backend.DTO.TicketReference;

public class TicketPriorityResponse
{
    public Guid PriorityId { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int PriorityLevel { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}