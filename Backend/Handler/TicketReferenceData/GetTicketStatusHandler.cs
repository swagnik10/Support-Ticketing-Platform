using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketStatusHandler
    : IRequestHandler<
        GetTicketStatusQuery,
        TicketStatusResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketStatusHandler> _logger;

    public GetTicketStatusHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketStatusHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketStatusResponse> Handle(
        GetTicketStatusQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket status {StatusId} for organization {OrganizationId}",
            query.StatusId,
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var status = await unitOfWork.Session
            .Query<TicketStatus>()
            .FirstOrDefaultAsync(
                x =>
                    x.StatusId == query.StatusId &&
                    x.OrganizationId == query.OrganizationId &&
                    x.IsActive,
                cancellationToken);

        if (status == null)
        {
            throw new KeyNotFoundException(
                $"Ticket status {query.StatusId} was not found.");
        }

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
