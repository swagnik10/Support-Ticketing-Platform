using Backend.CommandAndQuery;
using Backend.DTO.TicketComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:guid}/comments")]
public class TicketCommentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketCommentController> _logger;

    public TicketCommentController(
        IMediator mediator,
        ILogger<TicketCommentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TicketCommentResponse>> CreateComment(
        Guid ticketId,
        [FromBody] CreateTicketCommentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating comment for ticket {TicketId} in organization {OrganizationId}",
            ticketId,
            request.OrganizationId);

        var result = await _mediator.Send(
            new CreateTicketCommentCommand(ticketId, request),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpGet]
    public async Task<ActionResult<IList<TicketCommentResponse>>> GetComments(
        Guid ticketId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting comments for ticket {TicketId} in organization {OrganizationId}",
            ticketId,
            organizationId);

        var result = await _mediator.Send(
            new GetTicketCommentsQuery(ticketId, organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{commentId:guid}")]
    public async Task<ActionResult<TicketCommentResponse>> GetComment(
        Guid ticketId,
        Guid commentId,
        [FromQuery] Guid organizationId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting comment {CommentId} for ticket {TicketId}",
            commentId,
            ticketId);

        var result = await _mediator.Send(
            new GetTicketCommentQuery(
                ticketId,
                commentId,
                organizationId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{commentId:guid}")]
    public async Task<ActionResult<TicketCommentResponse>> UpdateComment(
        Guid ticketId,
        Guid commentId,
        [FromBody] UpdateTicketCommentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating comment {CommentId} for ticket {TicketId}",
            commentId,
            ticketId);

        var result = await _mediator.Send(
            new UpdateTicketCommentCommand(
                ticketId,
                commentId,
                request),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<ActionResult<bool>> DeleteComment(
        Guid ticketId,
        Guid commentId,
        [FromQuery] Guid organizationId,
        [FromQuery] Guid authorUserId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting comment {CommentId} from ticket {TicketId}",
            commentId,
            ticketId);

        var result = await _mediator.Send(
            new DeleteTicketCommentCommand(
                ticketId,
                commentId,
                organizationId,
                authorUserId),
            cancellationToken);

        return Ok(result);
    }
}