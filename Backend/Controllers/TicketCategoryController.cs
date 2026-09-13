using Backend.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/ticket-categories")]
public class TicketCategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketCategoryController> _logger;

    public TicketCategoryController(
        IMediator mediator,
        ILogger<TicketCategoryController> logger)
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
            "Received request to get ticket categories for organization {OrganizationId}",
            organizationId);

        var response = await _mediator.Send(
            new GetTicketCategoriesQuery(organizationId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetById(
        Guid categoryId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get ticket category {CategoryId} for organization {OrganizationId}",
            categoryId,
            organizationId);

        var response = await _mediator.Send(
            new GetTicketCategoryQuery(
                organizationId,
                categoryId),
            cancellationToken);

        return Ok(response);
    }
}
