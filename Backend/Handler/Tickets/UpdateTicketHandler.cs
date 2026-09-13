using System.Text.Json;
using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Ticket;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Tickets;

public class UpdateTicketHandler
    : IRequestHandler<UpdateTicketCommand, TicketResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateTicketHandler> _logger;

    public UpdateTicketHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateTicketHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketResponse> Handle(
        UpdateTicketCommand request,
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

            var ticket =
                await session.Query<Ticket>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.TicketId == request.TicketId &&
                            x.OrganizationId == data.OrganizationId &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket was not found.");

            // Actor must belong to the same organization.
            var actorMembership =
                await session.Query<OrganizationUser>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.OrganizationId == data.OrganizationId &&
                            x.UserId == data.ActorUserId &&
                            x.IsActive &&
                            x.DeletedAt == null,
                        cancellationToken);

            if (actorMembership == null)
                throw new InvalidOperationException(
                    "Actor user does not belong to the organization.");

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

            var oldValue = JsonSerializer.Serialize(new
            {
                subject = ticket.Subject,
                description = ticket.Description,
                categoryId = ticket.CategoryId,
                priorityId = ticket.PriorityId
            });

            ticket.Subject = data.Subject.Trim();
            ticket.Description = data.Description;
            ticket.CategoryId = data.CategoryId;
            ticket.PriorityId = data.PriorityId;
            ticket.UpdatedAt = DateTime.UtcNow;

            var newValue = JsonSerializer.Serialize(new
            {
                subject = ticket.Subject,
                description = ticket.Description,
                categoryId = ticket.CategoryId,
                priorityId = ticket.PriorityId
            });

            var ticketEvent = new TicketEvent
            {
                EventId = Guid.NewGuid(),
                OrganizationId = ticket.OrganizationId,
                TicketId = ticket.TicketId,
                EventType = "ticket.updated",
                ActorUserId = data.ActorUserId,
                OldValue = oldValue,
                NewValue = newValue,
                Metadata = JsonSerializer.Serialize(new
                {
                    source = "api"
                }),
                CorrelationId = Guid.NewGuid(),
                OccurredAt = ticket.UpdatedAt
            };

            await session.SaveAsync(ticketEvent, cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Ticket {TicketId} updated by user {ActorUserId}",
                ticket.TicketId,
                data.ActorUserId);

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