namespace Backend.Domain;

public class ProcessedEvent
{
    public virtual Guid ProcessedEventId { get; set; }

    public virtual string ConsumerName { get; set; } = string.Empty;

    public virtual Guid EventId { get; set; }

    public virtual DateTime ProcessedAt { get; set; }

    public virtual string? Metadata { get; set; }
}