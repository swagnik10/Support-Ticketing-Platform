namespace Backend.DTO.TicketAssignment;

public class TicketAssignmentResponse
{
    public Guid AssignmentId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid TicketId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public Guid AssignedByUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime? UnassignedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
