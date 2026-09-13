namespace Backend.DTO.TicketReference;

public class TicketStatusResponse
{
    public Guid StatusId { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string StatusCode { get; set; } = string.Empty;

    public bool IsInitial { get; set; }

    public bool IsTerminal { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
