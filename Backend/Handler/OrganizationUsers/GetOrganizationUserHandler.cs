using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.OrganizationUser;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.OrganizationUsers;

public class GetOrganizationUserHandler
    : IRequestHandler<
        GetOrganizationUserQuery,
        OrganizationUserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetOrganizationUserHandler> _logger;

    public GetOrganizationUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetOrganizationUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationUserResponse> Handle(
        GetOrganizationUserQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting organization user for organization {OrganizationId} and user {UserId}",
            query.OrganizationId,
            query.UserId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var organizationUser = await unitOfWork.Session
            .Query<OrganizationUser>()
            .FirstOrDefaultAsync(
                x => x.OrganizationId == query.OrganizationId
                     && x.UserId == query.UserId
                     && x.DeletedAt == null,
                cancellationToken);

        if (organizationUser == null)
        {
            throw new KeyNotFoundException(
                "Organization user association was not found.");
        }

        return new OrganizationUserResponse
        {
            OrganizationId = organizationUser.OrganizationId,
            UserId = organizationUser.UserId,
            RoleId = organizationUser.RoleId,
            IsActive = organizationUser.IsActive,
            JoinedAt = organizationUser.JoinedAt,
            CreatedAt = organizationUser.CreatedAt,
            UpdatedAt = organizationUser.UpdatedAt
        };
    }
}
