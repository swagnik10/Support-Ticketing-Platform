using Backend.DTO.Organization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Backend.CommandAndQuery.Orgnanization;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrganizationController> _logger;

    public OrganizationController(IMediator mediator, ILogger<OrganizationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to create organization with name {Name} and slug {Slug}",
            request.Name,
            request.Slug);
        var command = new CreateOrganizationCommand(request);

        var response = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.OrganizationId },
            response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get organization with id {Id}",
            id);

        var query = new GetOrganizationQuery(id);

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationsQuery();

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to update organization with id {Id}",
            id);

        var command = new UpdateOrganizationCommand(
            id,
            request);

        var response = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(response);
    }


    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrganizationStatusRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to update status of organization with id {Id}",
            id);

        var command = new UpdateOrganizationStatusCommand(
            id,
            request);

        var response = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(response);
    }
}
