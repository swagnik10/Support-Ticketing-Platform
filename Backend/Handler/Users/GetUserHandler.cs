using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.User;
using MediatR;

namespace Backend.Handler.Users;

public class GetUserHandler
    : IRequestHandler<GetUserQuery, UserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting user {UserId}",
            query.UserId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var user = await unitOfWork.Session
            .GetAsync<User>(
                query.UserId,
                cancellationToken);

        if (user == null || user.DeletedAt != null)
        {
            throw new KeyNotFoundException(
                $"User {query.UserId} was not found.");
        }

        return new UserResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName ?? "",
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}