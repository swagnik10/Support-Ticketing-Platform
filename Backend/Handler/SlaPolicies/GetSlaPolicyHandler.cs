using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.SlaPolicy;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.SlaPolicies;

public class GetSlaPolicyHandler
    : IRequestHandler<GetSlaPolicyQuery, SlaPolicyResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetSlaPolicyHandler> _logger;

    public GetSlaPolicyHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetSlaPolicyHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<SlaPolicyResponse> Handle(
        GetSlaPolicyQuery request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        var policy =
            await unitOfWork.Session.Query<SlaPolicy>()
                .FirstOrDefaultAsync(
                    x =>
                        x.SlaPolicyId == request.SlaPolicyId &&
                        x.OrganizationId == request.OrganizationId,
                    cancellationToken);

        if (policy == null)
            throw new KeyNotFoundException(
                "SLA policy was not found.");

        _logger.LogInformation(
            "Retrieved SLA policy {SlaPolicyId}",
            policy.SlaPolicyId);

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