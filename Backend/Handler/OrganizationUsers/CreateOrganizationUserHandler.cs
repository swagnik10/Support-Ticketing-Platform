using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.OrganizationUser;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.OrganizationUsers;

public class CreateOrganizationUserHandler
    : IRequestHandler<
        CreateOrganizationUserCommand,
        OrganizationUserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateOrganizationUserHandler> _logger;

    public CreateOrganizationUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateOrganizationUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationUserResponse> Handle(
        CreateOrganizationUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Adding user {UserId} to organization {OrganizationId} with role {RoleId}",
            command.Request.UserId,
            command.Request.OrganizationId,
            command.Request.RoleId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var organization = await unitOfWork.Session
                .GetAsync<Organization>(
                    command.Request.OrganizationId,
                    cancellationToken);

            if (organization == null || organization.DeletedAt != null)
            {
                throw new KeyNotFoundException(
                    $"Organization {command.Request.OrganizationId} was not found.");
            }

            var user = await unitOfWork.Session
                .GetAsync<User>(
                    command.Request.UserId,
                    cancellationToken);

            if (user == null || user.DeletedAt != null)
            {
                throw new KeyNotFoundException(
                    $"User {command.Request.UserId} was not found.");
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

            var existingOrganizationUser = await unitOfWork.Session
                .Query<OrganizationUser>()
                .FirstOrDefaultAsync(
                    x => x.OrganizationId == command.Request.OrganizationId
                         && x.UserId == command.Request.UserId,
                    cancellationToken);

            if (existingOrganizationUser != null)
            {
                if (existingOrganizationUser.DeletedAt == null)
                {
                    throw new InvalidOperationException(
                        "User is already associated with this organization.");
                }

                existingOrganizationUser.RoleId = command.Request.RoleId;
                existingOrganizationUser.IsActive = true;
                existingOrganizationUser.DeletedAt = null;
                existingOrganizationUser.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.Session.UpdateAsync(
                    existingOrganizationUser,
                    cancellationToken);

                await unitOfWork.CommitAsync();

                return MapToResponse(existingOrganizationUser);
            }

            var now = DateTime.UtcNow;

            var organizationUser = new OrganizationUser
            {
                OrganizationId = command.Request.OrganizationId,
                UserId = command.Request.UserId,
                RoleId = command.Request.RoleId,
                IsActive = true,
                JoinedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            };

            await unitOfWork.Session.SaveAsync(
                organizationUser,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "User {UserId} successfully added to organization {OrganizationId}",
                organizationUser.UserId,
                organizationUser.OrganizationId);

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