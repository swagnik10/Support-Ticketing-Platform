using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.SlaPolicy;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.SlaPolicies;

public class UpdateSlaPolicyHandler
    : IRequestHandler<UpdateSlaPolicyCommand, SlaPolicyResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateSlaPolicyHandler> _logger;

    public UpdateSlaPolicyHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateSlaPolicyHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<SlaPolicyResponse> Handle(
        UpdateSlaPolicyCommand request,
        CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.Create();

        try
        {
            unitOfWork.BeginTransaction();

            var policy =
                await unitOfWork.Session.Query<SlaPolicy>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.SlaPolicyId == request.SlaPolicyId &&
                            x.OrganizationId ==
                                request.Request.OrganizationId,
                        cancellationToken);

            if (policy == null)
                throw new KeyNotFoundException(
                    "SLA policy was not found.");

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
                            x.OrganizationId ==
                                request.Request.OrganizationId &&
                            x.SlaPolicyId != request.SlaPolicyId &&
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
                                x.SlaPolicyId != request.SlaPolicyId &&
                                x.IsDefault &&
                                x.IsActive)
                        .ToListAsync(cancellationToken);

                foreach (var existingPolicy in existingDefaults)
                {
                    existingPolicy.IsDefault = false;
                    existingPolicy.UpdatedAt = now;
                }
            }

            policy.Name = name;
            policy.Description = request.Request.Description?.Trim();
            policy.PriorityId = request.Request.PriorityId;
            policy.FirstResponseMinutes =
                request.Request.FirstResponseMinutes;
            policy.ResolutionMinutes =
                request.Request.ResolutionMinutes;
            policy.IsDefault = request.Request.IsDefault;
            policy.IsActive = request.Request.IsActive;
            policy.UpdatedAt = now;

            await unitOfWork.Session.FlushAsync(cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Updated SLA policy {SlaPolicyId}",
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
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}