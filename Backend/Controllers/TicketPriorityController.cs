using Backend.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/ticket-priorities")]
public class TicketPriorityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketPriorityController> _logger;

    public TicketPriorityController(
        IMediator mediator,
        ILogger<TicketPriorityController> logger)
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
            "Received request to get ticket priorities for organization {OrganizationId}",
            organizationId);

        var response = await _mediator.Send(
            new GetTicketPrioritiesQuery(organizationId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{priorityId:guid}")]
    public async Task<IActionResult> GetById(
        Guid priorityId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get ticket priority {PriorityId} for organization {OrganizationId}",
            priorityId,
            organizationId);

        var response = await _mediator.Send(
            new GetTicketPriorityQuery(
                organizationId,
                priorityId),
            cancellationToken);

        return Ok(response);
    }
}
