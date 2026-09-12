using Backend.DTO.User;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateUserCommand(
    CreateUserRequest Request
) : IRequest<UserResponse>;

public record GetUserQuery(
    Guid UserId
) : IRequest<UserResponse>;

public record GetUsersQuery
    : IRequest<IList<UserResponse>>;

public record UpdateUserCommand(
    Guid UserId,
    UpdateUserRequest Request
) : IRequest<UserResponse>;

public record UpdateUserStatusCommand(
    Guid UserId,
    UpdateUserStatusRequest Request
) : IRequest<UserResponse>;