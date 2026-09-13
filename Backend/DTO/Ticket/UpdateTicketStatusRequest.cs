namespace Backend.DTO.Ticket;

public class UpdateTicketStatusRequest
{
    public Guid OrganizationId { get; set; }

    public Guid StatusId { get; set; }

    public Guid ActorUserId { get; set; }
}
