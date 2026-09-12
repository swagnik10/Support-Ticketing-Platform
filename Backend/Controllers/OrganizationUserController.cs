using Backend.CommandAndQuery;
using Backend.DTO.OrganizationUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationUserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrganizationUserController> _logger;

    public OrganizationUserController(
        IMediator mediator,
        ILogger<OrganizationUserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrganizationUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to add user {UserId} to organization {OrganizationId}",
            request.UserId,
            request.OrganizationId);

        var response = await _mediator.Send(
            new CreateOrganizationUserCommand(request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                organizationId = response.OrganizationId,
                userId = response.UserId
            },
            response);
    }

    [HttpGet("{organizationId:guid}/{userId:guid}")]
    public async Task<IActionResult> GetById(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get user {UserId} from organization {OrganizationId}",
            userId,
            organizationId);

        var response = await _mediator.Send(
            new GetOrganizationUserQuery(
                organizationId,
                userId),
            cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to get organization users for organization {OrganizationId}",
            organizationId);

        var response = await _mediator.Send(
            new GetOrganizationUsersQuery(organizationId),
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{organizationId:guid}/{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid organizationId,
        Guid userId,
        [FromBody] UpdateOrganizationUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to update user {UserId} in organization {OrganizationId}",
            userId,
            organizationId);

        var response = await _mediator.Send(
            new UpdateOrganizationUserCommand(
                organizationId,
                userId,
                request),
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{organizationId:guid}/{userId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid organizationId,
        Guid userId,
        [FromBody] UpdateOrganizationUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received request to update status of user {UserId} in organization {OrganizationId}",
            userId,
            organizationId);

        var response = await _mediator.Send(
            new UpdateOrganizationUserStatusCommand(
                organizationId,
                userId,
                request),
            cancellationToken);

        return Ok(response);
    }
}