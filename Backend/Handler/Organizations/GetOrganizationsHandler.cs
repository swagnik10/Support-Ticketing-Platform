using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.Organization;
using MediatR;
using static Backend.CommandAndQuery.Orgnanization;
using NHibernate.Linq;

namespace Backend.Handler.Organizations;

public class GetOrganizationsHandler
    : IRequestHandler<
        GetOrganizationsQuery,
        IList<OrganizationResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetOrganizationsHandler> _logger;

    public GetOrganizationsHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetOrganizationsHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<OrganizationResponse>> Handle(
        GetOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting all organizations");

        using var unitOfWork = _unitOfWorkFactory.Create();

        var organizations =
            await unitOfWork.Session
                .Query<Organization>()
                .Where(x => x.DeletedAt == null)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

        return organizations
            .Select(MapToResponse)
            .ToList();
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
