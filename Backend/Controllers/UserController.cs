using Backend.CommandAndQuery;
using Backend.DTO.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating user with email {Email}",
            request.Email);

        var command = new CreateUserCommand(request);

        var response = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.UserId },
            response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting user with id {UserId}",
            id);

        var query = new GetUserQuery(id);

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users");

        var query = new GetUsersQuery();

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating user with id {UserId}",
            id);

        var command = new UpdateUserCommand(
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
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating status of user with id {UserId} to {IsActive}",
            id,
            request.IsActive);

        var command = new UpdateUserStatusCommand(
            id,
            request);

        var response = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(response);
    }
}
