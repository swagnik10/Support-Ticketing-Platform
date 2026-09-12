using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.OrganizationUser;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.OrganizationUsers;

public class GetOrganizationUsersHandler
    : IRequestHandler<
        GetOrganizationUsersQuery,
        IList<OrganizationUserResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetOrganizationUsersHandler> _logger;

    public GetOrganizationUsersHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetOrganizationUsersHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<OrganizationUserResponse>> Handle(
        GetOrganizationUsersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting organization users");

        using var unitOfWork = _unitOfWorkFactory.Create();

        var organizationUsersQuery = unitOfWork.Session
            .Query<OrganizationUser>()
            .Where(x => x.DeletedAt == null);

        if (query.OrganizationId.HasValue)
        {
            organizationUsersQuery = organizationUsersQuery
                .Where(x =>
                    x.OrganizationId == query.OrganizationId.Value);
        }

        var organizationUsers = await organizationUsersQuery
            .OrderBy(x => x.JoinedAt)
            .ToListAsync(cancellationToken);

        return organizationUsers
            .Select(x => new OrganizationUserResponse
            {
                OrganizationId = x.OrganizationId,
                UserId = x.UserId,
                RoleId = x.RoleId,
                IsActive = x.IsActive,
                JoinedAt = x.JoinedAt,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();
    }
}