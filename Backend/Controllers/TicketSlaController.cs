using Backend.CommandAndQuery;
using Backend.DTO.TicketSla;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:guid}/sla")]
public class TicketSlaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketSlaController> _logger;

    public TicketSlaController(
        IMediator mediator,
        ILogger<TicketSlaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<TicketSlaResponse>> GetTicketSla(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting SLA for ticket {TicketId}",
            ticketId);

        var result = await _mediator.Send(
            new GetTicketSlaQuery(
                ticketId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }
}