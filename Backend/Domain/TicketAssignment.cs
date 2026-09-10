namespace Backend.Domain;

public class TicketAssignment
{
    public virtual Guid AssignmentId { get; set; }

    public virtual Guid OrganizationId { get; set; }

    public virtual Guid TicketId { get; set; }

    public virtual Guid? AssignedToUserId { get; set; }

    public virtual Guid AssignedByUserId { get; set; }

    public virtual DateTime AssignedAt { get; set; }

    public virtual DateTime? UnassignedAt { get; set; }

    public virtual DateTime CreatedAt { get; set; }
}