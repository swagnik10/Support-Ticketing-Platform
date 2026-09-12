using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.User;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Users;

public class UpdateUserHandler
    : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<UpdateUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating user {UserId}",
            command.UserId);

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

            var existingUser = await unitOfWork.Session
                .Query<User>()
                .FirstOrDefaultAsync(
                    x => x.Email == command.Request.Email
                         && x.UserId != command.UserId
                         && x.DeletedAt == null,
                    cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    $"A user with email '{command.Request.Email}' already exists.");
            }

            user.Email = command.Request.Email;
            user.FirstName = command.Request.FirstName;
            user.LastName = command.Request.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Session.UpdateAsync(
                user,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "User {UserId} updated successfully",
                command.UserId);

            return new UserResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
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
