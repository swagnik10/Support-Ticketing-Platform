using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketPrioritiesHandler
    : IRequestHandler<
        GetTicketPrioritiesQuery,
        IList<TicketPriorityResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketPrioritiesHandler> _logger;

    public GetTicketPrioritiesHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketPrioritiesHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketPriorityResponse>> Handle(
        GetTicketPrioritiesQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket priorities for organization {OrganizationId}",
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var priorities = await unitOfWork.Session
            .Query<TicketPriority>()
            .Where(x =>
                x.OrganizationId == query.OrganizationId &&
                x.IsActive)
            .OrderBy(x => x.PriorityLevel)
            .ToListAsync(cancellationToken);

        return priorities
            .Select(MapToResponse)
            .ToList();
    }

    private static TicketPriorityResponse MapToResponse(
        TicketPriority priority)
    {
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
