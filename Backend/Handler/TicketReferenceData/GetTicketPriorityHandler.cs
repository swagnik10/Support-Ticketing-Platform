using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketPriorityHandler
    : IRequestHandler<
        GetTicketPriorityQuery,
        TicketPriorityResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketPriorityHandler> _logger;

    public GetTicketPriorityHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketPriorityHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketPriorityResponse> Handle(
        GetTicketPriorityQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket priority {PriorityId} for organization {OrganizationId}",
            query.PriorityId,
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var priority = await unitOfWork.Session
            .Query<TicketPriority>()
            .FirstOrDefaultAsync(
                x =>
                    x.PriorityId == query.PriorityId &&
                    x.OrganizationId == query.OrganizationId &&
                    x.IsActive,
                cancellationToken);

        if (priority == null)
        {
            throw new KeyNotFoundException(
                $"Ticket priority {query.PriorityId} was not found.");
        }

        return new TicketPriorityResponse
        {
            PriorityId = priority.PriorityId,
            OrganizationId = priority.OrganizationId,
            Name = priority.Name,
            PriorityLevel = priority.PriorityLevel,
            IsActive = priority.IsActive,
            CreatedAt = priority.CreatedAt,
            UpdatedAt = priority.UpdatedAt
        };
    }
}
