namespace Backend.DTO.Ticket;

public class CreateTicketRequest
{
    public Guid OrganizationId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid CustomerUserId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid PriorityId { get; set; }

    public Guid CreatedByUserId { get; set; }
}