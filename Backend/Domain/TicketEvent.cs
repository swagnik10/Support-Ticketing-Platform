namespace Backend.Domain;

public class TicketEvent
{
    public virtual Guid EventId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual Guid TicketId { get; set; }

    public virtual string EventType { get; set; } = string.Empty;

    public virtual Guid? ActorUserId { get; set; }

    public virtual string? OldValue { get; set; }

    public virtual string? NewValue { get; set; }

    public virtual string? Metadata { get; set; }

    public virtual Guid? CorrelationId { get; set; }

    public virtual DateTime OccurredAt { get; set; }
}