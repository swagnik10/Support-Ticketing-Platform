using Backend.CommandAndQuery;
using Backend.DbConnection;
using Backend.Domain;
using Backend.DTO.TicketReference;
using MediatR;
using NHibernate.Linq;

namespace Backend.Handler.TicketReferenceData;

public class GetTicketCategoriesHandler
    : IRequestHandler<
        GetTicketCategoriesQuery,
        IList<TicketCategoryResponse>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GetTicketCategoriesHandler> _logger;

    public GetTicketCategoriesHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<GetTicketCategoriesHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task<IList<TicketCategoryResponse>> Handle(
        GetTicketCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting ticket categories for organization {OrganizationId}",
            query.OrganizationId);

        using var unitOfWork = _unitOfWorkFactory.Create();

        var categories = await unitOfWork.Session
            .Query<TicketCategory>()
            .Where(x =>
                x.OrganizationId == query.OrganizationId &&
                x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return categories
            .Select(MapToResponse)
            .ToList();
    }

    private static TicketCategoryResponse MapToResponse(
        TicketCategory category)
    {
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