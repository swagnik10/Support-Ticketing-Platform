using Backend.CommandAndQuery;
using Backend.DTO.TicketAssignment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:guid}/assignment")]
public class TicketAssignmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketAssignmentController> _logger;

    public TicketAssignmentController(
        IMediator mediator,
        ILogger<TicketAssignmentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TicketAssignmentResponse>> AssignTicket(
        Guid ticketId,
        [FromBody] AssignTicketRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Assigning ticket {TicketId} to user {AssignedToUserId} for organization {OrganizationId}",
            ticketId,
            request.AssignedToUserId,
            request.OrganizationId);

        var result = await _mediator.Send(
            new AssignTicketCommand(ticketId, request),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpGet]
    public async Task<ActionResult<TicketAssignmentResponse>> GetTicketAssignment(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting assignment for ticket {TicketId} for organization {OrganizationId}",
            ticketId,
            organizationId);

        var result = await _mediator.Send(
            new GetTicketAssignmentQuery(
                ticketId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<TicketAssignmentResponse>> ReassignTicket(
        Guid ticketId,
        [FromBody] AssignTicketRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Reassigning ticket {TicketId} to user {AssignedToUserId} for organization {OrganizationId}",
            ticketId,
            request.AssignedToUserId,
            request.OrganizationId);

        var result = await _mediator.Send(
            new ReassignTicketCommand(ticketId, request),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> UnassignTicket(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        [FromQuery] Guid assignedByUserId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Unassigning ticket {TicketId} for organization {OrganizationId}",
            ticketId,
            organizationId);

        await _mediator.Send(
            new UnassignTicketCommand(
                ticketId,
                organizationId,
                assignedByUserId),
            cancellationToken);

        return NoContent();
    }
}