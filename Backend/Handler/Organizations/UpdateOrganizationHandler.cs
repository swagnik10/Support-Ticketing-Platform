using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Organization;
using MediatR;
using Backend.CommandAndQuery;

namespace Backend.Handler.Organizations;

public class UpdateOrganizationHandler
    : IRequestHandler<UpdateOrganizationCommand, OrganizationResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateOrganizationHandler> _logger;

    public UpdateOrganizationHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateOrganizationHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationResponse> Handle(
        UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating organization {OrganizationId}",
            command.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var organization =
                await unitOfWork.Session.GetAsync<Organization>(
                    command.OrganizationId,
                    cancellationToken);

            if (organization == null)
            {
                throw new KeyNotFoundException(
                    $"Organization {command.OrganizationId} was not found.");
            }

            organization.Name = command.Request.Name;
            organization.Slug = command.Request.Slug;
            organization.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.UpdateAsync(
                organization,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Organization {OrganizationId} updated successfully",
                command.OrganizationId);

            return MapToResponse(organization);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static OrganizationResponse MapToResponse(
        Organization organization)
    {
        return new OrganizationResponse
        {
            OrganizationId = organization.OrganizationId,
            Name = organization.Name,
            Slug = organization.Slug,
            IsActive = organization.IsActive,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };
    }
}
