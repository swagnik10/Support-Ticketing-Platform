namespace Backend.DTO.Ticket;

public class UpdateTicketRequest
{
    public Guid OrganizationId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Guid PriorityId { get; set; }

    public Guid ActorUserId { get; set; }
}
