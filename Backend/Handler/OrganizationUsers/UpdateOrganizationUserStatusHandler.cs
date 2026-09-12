using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.OrganizationUser;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.OrganizationUsers;

public class UpdateOrganizationUserStatusHandler
    : IRequestHandler<
        UpdateOrganizationUserStatusCommand,
        OrganizationUserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateOrganizationUserStatusHandler> _logger;

    public UpdateOrganizationUserStatusHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateOrganizationUserStatusHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<OrganizationUserResponse> Handle(
        UpdateOrganizationUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating status of user {UserId} in organization {OrganizationId} to {IsActive}",
            command.UserId,
            command.OrganizationId,
            command.Request.IsActive);

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

            organizationUser.IsActive = command.Request.IsActive;
            organizationUser.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.UpdateAsync(
                organizationUser,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Status updated for user {UserId} in organization {OrganizationId}",
                command.UserId,
                command.OrganizationId);

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
