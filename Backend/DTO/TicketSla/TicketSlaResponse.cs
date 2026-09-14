namespace Backend.DTO.TicketSla;

public class TicketSlaResponse
{
    public Guid TicketSlaId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid TicketId { get; set; }

    public Guid SlaPolicyId { get; set; }

    public DateTime? FirstResponseDueAt { get; set; }

    public DateTime? FirstRespondedAt { get; set; }

    public DateTime? ResolutionDueAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public bool FirstResponseBreached { get; set; }

    public bool ResolutionBreached { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
