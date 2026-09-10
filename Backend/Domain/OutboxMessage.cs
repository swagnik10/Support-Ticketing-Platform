namespace Backend.Domain;

public class OutboxMessage
{
    public virtual Guid OutboxMessageId { get; set; }

    public virtual Guid? OrganizationId { get; set; }

    public virtual string AggregateType { get; set; } = string.Empty;

    public virtual Guid AggregateId { get; set; }

    public virtual string EventType { get; set; } = string.Empty;

    public virtual string Payload { get; set; } = string.Empty;

    public virtual Guid? CorrelationId { get; set; }

    public virtual DateTime OccurredAt { get; set; }

    public virtual DateTime? PublishedAt { get; set; }

    public virtual int RetryCount { get; set; }

    public virtual string? LastError { get; set; }

    public virtual DateTime? LockedAt { get; set; }

    public virtual DateTime CreatedAt { get; set; }
}