using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.User;
using MediatR;

namespace Backend.Handler.Users;

public class UpdateUserStatusHandler
    : IRequestHandler<UpdateUserStatusCommand, UserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateUserStatusHandler> _logger;

    public UpdateUserStatusHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateUserStatusHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(
        UpdateUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating status of user {UserId} to {IsActive}",
            command.UserId,
            command.Request.IsActive);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var user = await unitOfWork.Session
                .GetAsync<User>(
                    command.UserId,
                    cancellationToken);

            if (user == null || user.DeletedAt != null)
            {
                throw new KeyNotFoundException(
                    $"User {command.UserId} was not found.");
            }

            user.IsActive = command.Request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.UpdateAsync(
                user,
                cancellationToken);

            await unitOfWork.CommitAsync();

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
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}