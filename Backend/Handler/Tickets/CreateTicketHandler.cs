using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Ticket;
using Backend.Services;
using MediatR;
using NHibernate.Linq;
using System.Text.Json;

namespace Backend.Handler.Tickets;

public class CreateTicketHandler
    : IRequestHandler<CreateTicketCommand, TicketResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateTicketHandler> _logger;
    private readonly OutboxService _outboxService;

    public CreateTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateTicketHandler> logger,
        OutboxService outboxService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
        _outboxService = outboxService;
    }

    public async Task<TicketResponse> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        var data = request.Request;

        if (string.IsNullOrWhiteSpace(data.Subject))
            throw new ArgumentException("Subject is required.");

        if (string.IsNullOrWhiteSpace(data.Description))
            throw new ArgumentException("Description is required.");

        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var session = unitOfWork.Session;

            // Customer must belong to the organization.
            var customerMembership =
                await session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == data.OrganizationId &&
                            x.UserId == data.CustomerUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (customerMembership == null)
                throw new InvalidOperationException(
                    "Customer user does not belong to the organization.");

            // Creator must belong to the organization.
            var creatorMembership =
                await session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == data.OrganizationId &&
                            x.UserId == data.CreatedByUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (creatorMembership == null)
                throw new InvalidOperationException(
                    "CreatedByUser does not belong to the organization.");

            // Category must belong to organization and be active.
            var category =
                await session.Query<TicketCategory>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.CategoryId == data.CategoryId &&
                            x.OrganizationId == data.OrganizationId &&
                            x.IsActive,
                        cancellationToken);

            if (category == null)
                throw new InvalidOperationException(
                    "Ticket category was not found or is inactive.");

            // Priority must belong to organization and be active.
            var priority =
                await session.Query<TicketPriority>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.PriorityId == data.PriorityId &&
                            x.OrganizationId == data.OrganizationId &&
                            x.IsActive,
                        cancellationToken);

            if (priority == null)
                throw new InvalidOperationException(
                    "Ticket priority was not found or is inactive.");

            // Pick organization's active initial status.
            var initialStatus =
                await session.Query<TicketStatus>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == data.OrganizationId &&
                            x.IsActive &&
                            x.IsInitial,
                        cancellationToken);

            if (initialStatus == null)
                throw new InvalidOperationException(
                    "No active initial ticket status is configured for the organization.");

            var now = DateTime.UtcNow;

            var ticket = new Ticket
            {
                OrganizationId = data.OrganizationId,
                Subject = data.Subject.Trim(),
                Description = data.Description,
                CustomerUserId = data.CustomerUserId,
                AssignedAgentId = null,
                CategoryId = data.CategoryId,
                PriorityId = data.PriorityId,
                StatusId = initialStatus.StatusId,
                CreatedByUserId = data.CreatedByUserId,
                CreatedAt = now,
                UpdatedAt = now,
                ResolvedAt = null,
                ClosedAt = null,
                DeletedAt = null
            };

            await session.SaveAsync(ticket, cancellationToken);

            // Make sure the database-generated ticket_number is available.
            await session.FlushAsync(cancellationToken);
            await session.RefreshAsync(ticket, cancellationToken);

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = ticket.OrganizationId,
                TicketId = ticket.TicketId,
                EventType = "ticket.created",
                ActorUserId = data.CreatedByUserId,
                OldValue = null,
                NewValue = JsonSerializer.Serialize(new
                {
                    ticketId = ticket.TicketId,
                    ticketNumber = ticket.TicketNumber,
                    subject = ticket.Subject,
                    statusId = ticket.StatusId
                }),
                Metadata = JsonSerializer.Serialize(new
                {
                    source = "api"
                }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = now
            };

            await session.SaveAsync(ticketEvent, cancellationToken);

            await _outboxService.AddAsync(ticketEvent, cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} with ticket number {TicketNumber} created for organization {OrganizationId}",
                ticket.TicketId,
                ticket.TicketNumber,
                ticket.OrganizationId);

            return MapToResponse(ticket);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static TicketResponse MapToResponse(Ticket ticket)
    {
        return new TicketResponse
        {
            TicketId = ticket.TicketId,
            OrganizationId = ticket.OrganizationId,
            TicketNumber = ticket.TicketNumber,
            Subject = ticket.Subject,
            Description = ticket.Description,
            CustomerUserId = ticket.CustomerUserId,
            AssignedAgentId = ticket.AssignedAgentId,
            CategoryId = ticket.CategoryId,
            PriorityId = ticket.PriorityId,
            StatusId = ticket.StatusId,
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt
        };
    }
}