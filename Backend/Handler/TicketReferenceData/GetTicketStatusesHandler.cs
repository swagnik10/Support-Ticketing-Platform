using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketStatusesHandler
    : IRequestHandler<
        GetTicketStatusesQuery,
        IList<TicketStatusResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketStatusesHandler> _logger;

    public GetTicketStatusesHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketStatusesHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketStatusResponse>> Handle(
        GetTicketStatusesQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket statuses for organization {OrganizationId}",
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var statuses = await unitOfWork.Session
            .Query<TicketStatus>()
            .Where(x =>
                x.OrganizationId == query.OrganizationId &&
                x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return statuses
            .Select(MapToResponse)
            .ToList();
    }

    private static TicketStatusResponse MapToResponse(
        TicketStatus status)
    {
        return new TicketStatusResponse
        {
            StatusId = status.StatusId,
            OrganizationId = status.OrganizationId,
            Name = status.Name,
            StatusCode = status.StatusCode,
            IsInitial = status.IsInitial,
            IsTerminal = status.IsTerminal,
            IsActive = status.IsActive,
            CreatedAt = status.CreatedAt,
            UpdatedAt = status.UpdatedAt
        };
    }
}
