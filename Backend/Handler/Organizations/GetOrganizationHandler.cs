using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Organization;
using MediatR;
using static Backend.CommandAndQuery.Orgnanization;

namespace Backend.Handler.Organizations;

public class GetOrganizationHandler
    : IRequestHandler<GetOrganizationQuery, OrganizationResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetOrganizationHandler> _logger;

    public GetOrganizationHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetOrganizationHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationResponse> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting organization {OrganizationId}",
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var organization =
            await unitOfWork.Session.GetAsync<Organization>(
                query.OrganizationId,
                cancellationToken);

        if (organization == null)
        {
            throw new KeyNotFoundException(
                $"Organization {query.OrganizationId} was not found.");
        }

        return MapToResponse(organization);
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

