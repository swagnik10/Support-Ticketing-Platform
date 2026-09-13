using Backend.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/ticket-statuses")]
public class TicketStatusController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketStatusController> _logger;

    public TicketStatusController(
        IMediator mediator,
        ILogger<TicketStatusController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get ticket statuses for organization {OrganizationId}",
            organizationId);

        var response = await _mediator.Send(
            new GetTicketStatusesQuery(organizationId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{statusId:guid}")]
    public async Task<IActionResult> GetById(
        Guid statusId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get ticket status {StatusId} for organization {OrganizationId}",
            statusId,
            organizationId);

        var response = await _mediator.Send(
            new GetTicketStatusQuery(
                organizationId,
                statusId),
            cancellationToken);

        return Ok(response);
    }
}
