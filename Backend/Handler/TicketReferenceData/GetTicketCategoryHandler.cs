using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketCategoryHandler
    : IRequestHandler<
        GetTicketCategoryQuery,
        TicketCategoryResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketCategoryHandler> _logger;

    public GetTicketCategoryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketCategoryHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<TicketCategoryResponse> Handle(
        GetTicketCategoryQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket category {CategoryId} for organization {OrganizationId}",
            query.CategoryId,
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var category = await unitOfWork.Session
            .Query<TicketCategory>()
            .FirstOrDefaultAsync(
                x =>
                    x.CategoryId == query.CategoryId &&
                    x.OrganizationId == query.OrganizationId &&
                    x.IsActive,
                cancellationToken);

        if (category == null)
        {
            throw new KeyNotFoundException(
                $"Ticket category {query.CategoryId} was not found.");
        }

        return new TicketCategoryResponse
        {
            CategoryId = category.CategoryId,
            OrganizationId = category.OrganizationId,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
