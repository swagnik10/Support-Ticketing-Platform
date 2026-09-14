using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.SlaPolicy;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.SlaPolicies;

public class CreateSlaPolicyHandler
    : IRequestHandler<CreateSlaPolicyCommand, SlaPolicyResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateSlaPolicyHandler> _logger;

    public CreateSlaPolicyHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateSlaPolicyHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<SlaPolicyResponse> Handle(
        CreateSlaPolicyCommand request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var organizationExists =
                await unitOfWork.Session.Query<Organization>()
                    .AnyAsync(
                        x =>
                            x.OrganizationId == request.Request.OrganizationId &&
                            x.DeletedAt == null &&
                            x.IsActive,
                        cancellationToken);

            if (!organizationExists)
                throw new KeyNotFoundException(
                    "Organization was not found.");

            var name = request.Request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "SLA policy name is required.");

            if (request.Request.FirstResponseMinutes <= 0)
                throw new ArgumentException(
                    "First response minutes must be greater than zero.");

            if (request.Request.ResolutionMinutes <= 0)
                throw new ArgumentException(
                    "Resolution minutes must be greater than zero.");

            var duplicateExists =
                await unitOfWork.Session.Query<SlaPolicy>()
                    .AnyAsync(
                        x =>
                            x.OrganizationId == request.Request.OrganizationId &&
                            x.Name == name,
                        cancellationToken);

            if (duplicateExists)
                throw new InvalidOperationException(
                    "An SLA policy with the same name already exists.");

            var now = DateTime.UtcNow;

            if (request.Request.IsDefault)
            {
                var existingDefaults =
                    await unitOfWork.Session.Query<SlaPolicy>()
                        .Where(
                            x =>
                                x.OrganizationId ==
                                    request.Request.OrganizationId &&
                                x.IsDefault &&
                                x.IsActive)
                        .ToListAsync(cancellationToken);

                foreach (var pol in existingDefaults)
                {
                    pol.IsDefault = false;
                    pol.UpdatedAt = now;
                }
            }

            var policy = new SlaPolicy
            {
                SlaPolicyId = Guid.NewGuid(),
                OrganizationId = request.Request.OrganizationId,
                Name = name,
                Description = request.Request.Description?.Trim(),
                PriorityId = request.Request.PriorityId,
                FirstResponseMinutes =
                    request.Request.FirstResponseMinutes,
                ResolutionMinutes =
                    request.Request.ResolutionMinutes,
                IsDefault = request.Request.IsDefault,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            await unitOfWork.Session.SaveAsync(
                policy,
                cancellationToken);

            await unitOfWork.Session.FlushAsync(cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Created SLA policy {SlaPolicyId} for organization {OrganizationId}",
                policy.SlaPolicyId,
                policy.OrganizationId);

            return MapToResponse(policy);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
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