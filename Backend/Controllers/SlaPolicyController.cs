using Backend.CommandAndQuery;
using Backend.DTO.SlaPolicy;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/sla-policies")]
public class SlaPolicyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SlaPolicyController> _logger;

    public SlaPolicyController(
        IMediator mediator,
        ILogger<SlaPolicyController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<SlaPolicyResponse>> CreateSlaPolicy(
        [FromBody] CreateSlaPolicyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating SLA policy for organization {OrganizationId}",
            request.OrganizationId);

        var result = await _mediator.Send(
            new CreateSlaPolicyCommand(request),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpGet]
    public async Task<ActionResult<IList<SlaPolicyResponse>>> GetSlaPolicies(
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting SLA policies for organization {OrganizationId}",
            organizationId);

        var result = await _mediator.Send(
            new GetSlaPoliciesQuery(organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{slaPolicyId:guid}")]
    public async Task<ActionResult<SlaPolicyResponse>> GetSlaPolicy(
        Guid slaPolicyId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSlaPolicyQuery(
                slaPolicyId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{slaPolicyId:guid}")]
    public async Task<ActionResult<SlaPolicyResponse>> UpdateSlaPolicy(
        Guid slaPolicyId,
        [FromBody] UpdateSlaPolicyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating SLA policy {SlaPolicyId}",
            slaPolicyId);

        var result = await _mediator.Send(
            new UpdateSlaPolicyCommand(
                slaPolicyId,
                request),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{slaPolicyId:guid}")]
    public async Task<ActionResult<bool>> DeactivateSlaPolicy(
        Guid slaPolicyId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deactivating SLA policy {SlaPolicyId}",
            slaPolicyId);

        var result = await _mediator.Send(
            new DeactivateSlaPolicyCommand(
                slaPolicyId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }
}