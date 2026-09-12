using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.User;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.Users;

public class GetUsersHandler
    : IRequestHandler<GetUsersQuery, IList<UserResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetUsersHandler> _logger;

    public GetUsersHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetUsersHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<UserResponse>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users");

        using var unitOfWork = _unitOfWorkFactory.Create();

        var users = await unitOfWork.Session
            .Query<User>()
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync(cancellationToken);

        return users
            .Select(user => new UserResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName ?? "",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .ToList();
    }
}
