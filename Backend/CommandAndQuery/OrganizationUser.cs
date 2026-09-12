using Backend.DTO.OrganizationUser;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateOrganizationUserCommand(
    CreateOrganizationUserRequest Request
) : IRequest<OrganizationUserResponse>;

public record GetOrganizationUserQuery(
    Guid OrganizationId,
    Guid UserId
) : IRequest<OrganizationUserResponse>;

public record GetOrganizationUsersQuery(
    Guid? OrganizationId
) : IRequest<IList<OrganizationUserResponse>>;

public record UpdateOrganizationUserCommand(
    Guid OrganizationId,
    Guid UserId,
    UpdateOrganizationUserRequest Request
) : IRequest<OrganizationUserResponse>;

public record UpdateOrganizationUserStatusCommand(
    Guid OrganizationId,
    Guid UserId,
    UpdateOrganizationUserStatusRequest Request
) : IRequest<OrganizationUserResponse>;
