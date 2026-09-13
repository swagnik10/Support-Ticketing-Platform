using Backend.CommandAndQuery;
using Backend.DTO.Ticket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tickets")]
public class SupportTicketController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SupportTicketController> _logger;

    public SupportTicketController(
        IMediator mediator,
        ILogger<SupportTicketController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> CreateTicket(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating ticket for organization {OrganizationId}",
            request.OrganizationId);

        var result = await _mediator.Send(
            new CreateTicketCommand(request),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    public async Task<ActionResult<IList<TicketResponse>>> GetTickets(
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting tickets for organization {OrganizationId}",
            organizationId);

        var result = await _mediator.Send(
            new GetTicketsQuery(organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{ticketId:guid}")]
    public async Task<ActionResult<TicketResponse>> GetTicket(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket {TicketId} for organization {OrganizationId}",
            ticketId,
            organizationId);

        var result = await _mediator.Send(
            new GetTicketQuery(organizationId, ticketId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{ticketId:guid}")]
    public async Task<ActionResult<TicketResponse>> UpdateTicket(
        Guid ticketId,
        [FromBody] UpdateTicketRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating ticket {TicketId} for organization {OrganizationId}",
            ticketId,
            request.OrganizationId);

        var result = await _mediator.Send(
            new UpdateTicketCommand(ticketId, request),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{ticketId:guid}/status")]
    public async Task<ActionResult<TicketResponse>> UpdateTicketStatus(
        Guid ticketId,
        [FromBody] UpdateTicketStatusRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Changing status of ticket {TicketId} to {StatusId}",
            ticketId,
            request.StatusId);

        var result = await _mediator.Send(
            new UpdateTicketStatusCommand(ticketId, request),
            cancellationToken);

        return Ok(result);
    }
}