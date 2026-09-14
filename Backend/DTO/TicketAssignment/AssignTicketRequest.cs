namespace Backend.DTO.TicketAssignment;

public class AssignTicketRequest
{
    public Guid OrganizationId { get; set; }

    public Guid AssignedToUserId { get; set; }

    public Guid AssignedByUserId { get; set; }
}
