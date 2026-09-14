using Backend.CommandAndQuery;
using Backend.DTO.TicketEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:guid}/events")]
public class TicketEventController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketEventController> _logger;

    public TicketEventController(
        IMediator mediator,
        ILogger<TicketEventController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IList<TicketEventResponse>>> GetEvents(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting events for ticket {TicketId} in organization {OrganizationId}",
            ticketId,
            organizationId);

        var result = await _mediator.Send(
            new GetTicketEventsQuery(
                ticketId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }
}