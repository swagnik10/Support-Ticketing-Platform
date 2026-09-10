namespace Backend.Domain;

public class TicketSla
{
    public virtual Guid TicketSlaId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual Guid TicketId { get; set; }

    public virtual Guid SlaPolicyId { get; set; }

    public virtual DateTime? FirstResponseDueAt { get; set; }

    public virtual DateTime? FirstRespondedAt { get; set; }

    public virtual DateTime? ResolutionDueAt { get; set; }

    public virtual DateTime? ResolvedAt { get; set; }

    public virtual bool FirstResponseBreached { get; set; }

    public virtual bool ResolutionBreached { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }
}