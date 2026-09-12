using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.OrganizationUser;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.OrganizationUsers;

public class UpdateOrganizationUserHandler
    : IRequestHandler<
        UpdateOrganizationUserCommand,
        OrganizationUserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateOrganizationUserHandler> _logger;

    public UpdateOrganizationUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateOrganizationUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationUserResponse> Handle(
        UpdateOrganizationUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating organization user {UserId} in organization {OrganizationId}",
            command.UserId,
            command.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var organizationUser = await unitOfWork.Session
                .Query<OrganizationUser>()
                .FirstOrDefaultAsync(
                    x => x.OrganizationId == command.OrganizationId
                         && x.UserId == command.UserId
                         && x.DeletedAt == null,
                    cancellationToken);

            if (organizationUser == null)
            {
                throw new KeyNotFoundException(
                    "Organization user association was not found.");
            }

            var role = await unitOfWork.Session
                .GetAsync<Role>(
                    command.Request.RoleId,
                    cancellationToken);

            if (role == null)
            {
                throw new KeyNotFoundException(
                    $"Role {command.Request.RoleId} was not found.");
            }

            organizationUser.RoleId = command.Request.RoleId;
            organizationUser.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.UpdateAsync(
                organizationUser,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Organization user {UserId} role updated successfully",
                command.UserId);

            return MapToResponse(organizationUser);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static OrganizationUserResponse MapToResponse(
        OrganizationUser organizationUser)
    {
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