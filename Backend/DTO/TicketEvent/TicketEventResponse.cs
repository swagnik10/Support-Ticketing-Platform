namespace Backend.DTO.TicketEvent;

public class TicketEventResponse
{
    public Guid EventId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid TicketId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public Guid? ActorUserId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Metadata { get; set; }

    public Guid? CorrelationId { get; set; }

    public DateTime OccurredAt { get; set; }
}