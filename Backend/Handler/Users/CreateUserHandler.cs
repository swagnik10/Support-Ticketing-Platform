using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.User;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Users;

public class CreateUserHandler
    : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CreateUserHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating user with email {Email}",
            command.Request.Email);

        using var unitOfWork = _unitOfWorkFactory.Create();

        unitOfWork.BeginTransaction();

        try
        {
            var existingUser = await unitOfWork.Session
                .Query<User>()
                .FirstOrDefaultAsync(
                    x => x.Email == command.Request.Email
                         && x.DeletedAt == null,
                    cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    $"A user with email '{command.Request.Email}' already exists.");
            }

            var user = new User
            {
                Email = command.Request.Email,
                FirstName = command.Request.FirstName,
                LastName = command.Request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Session.SaveAsync(
                user,
                cancellationToken);

            await unitOfWork.CommitAsync();

            _logger.LogInformation(
                "User {UserId} created successfully",
                user.UserId);

            return MapToResponse(user);
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }

    private static UserResponse MapToResponse(User user)
    {
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
}
