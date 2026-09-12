using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Organization;
using MediatR;


namespace Backend.Handler.Organizations;

public class CreateOrganizationHandler
    : IRequestHandler<CreateOrganizationCommand, OrganizationResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateOrganizationHandler> _logger;

    public CreateOrganizationHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateOrganizationHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationResponse> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating organization with name {Name} and slug {Slug}",
            command.Request.Name,
            command.Request.Slug);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var organization = new Organization
            {
                Name = command.Request.Name,
                Slug = command.Request.Slug,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Session.SaveAsync(
                organization,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Organization {OrganizationId} created successfully",
                organization.OrganizationId);

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