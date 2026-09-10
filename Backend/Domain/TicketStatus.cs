namespace Backend.Domain;

public class TicketStatus
{
    public virtual Guid StatusId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual string Name { get; set; } = string.Empty;

    public virtual string StatusCode { get; set; } = string.Empty;

    public virtual bool IsInitial { get; set; }

    public virtual bool IsTerminal { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime UpdatedAt { get; set; }
}