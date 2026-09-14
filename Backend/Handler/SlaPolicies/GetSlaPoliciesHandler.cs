using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.SlaPolicy;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.SlaPolicies;

public class GetSlaPoliciesHandler
    : IRequestHandler<GetSlaPoliciesQuery, IList<SlaPolicyResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetSlaPoliciesHandler> _logger;

    public GetSlaPoliciesHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetSlaPoliciesHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<SlaPolicyResponse>> Handle(
        GetSlaPoliciesQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var policies =
            await unitOfWork.Session.Query<SlaPolicy>()
                .Where(
                    x =>
                        x.OrganizationId == request.OrganizationId &&
                        x.IsActive)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {PolicyCount} SLA policies for organization {OrganizationId}",
            policies.Count,
            request.OrganizationId);

        return policies
            .Select(MapToResponse)
            .ToList();
    }

    private static SlaPolicyResponse MapToResponse(
        SlaPolicy policy)
    {
        return new SlaPolicyResponse
        {
            SlaPolicyId = policy.SlaPolicyId,
            OrganizationId = policy.OrganizationId,
            Name = policy.Name,
            Description = policy.Description,
            PriorityId = policy.PriorityId,
            FirstResponseMinutes = policy.FirstResponseMinutes,
            ResolutionMinutes = policy.ResolutionMinutes,
            IsDefault = policy.IsDefault,
            IsActive = policy.IsActive,
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt
        };
    }
}