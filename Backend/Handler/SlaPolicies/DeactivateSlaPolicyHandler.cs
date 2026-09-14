using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.SlaPolicies;

public class DeactivateSlaPolicyHandler
    : IRequestHandler<DeactivateSlaPolicyCommand, bool>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<DeactivateSlaPolicyHandler> _logger;

    public DeactivateSlaPolicyHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<DeactivateSlaPolicyHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeactivateSlaPolicyCommand request,
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
                            x.OrganizationId == request.OrganizationId,
                        cancellationToken);

            if (policy == null)
                throw new KeyNotFoundException(
                    "SLA policy was not found.");

            if (!policy.IsActive)
                return true;

            policy.IsActive = false;
            policy.IsDefault = false;
            policy.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.FlushAsync(cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Deactivated SLA policy {SlaPolicyId}",
                policy.SlaPolicyId);

            return true;
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}