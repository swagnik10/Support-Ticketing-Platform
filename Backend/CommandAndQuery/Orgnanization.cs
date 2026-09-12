using Backend.DTO.Organization;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateOrganizationCommand(
CreateOrganizationRequest Request
) : IRequest<OrganizationResponse>;

public record UpdateOrganizationCommand(
    Guid OrganizationId,
    UpdateOrganizationRequest Request
) : IRequest<OrganizationResponse>;

public record UpdateOrganizationStatusCommand(
    Guid OrganizationId,
    UpdateOrganizationStatusRequest Request
) : IRequest<OrganizationResponse>;

public record GetOrganizationQuery(
Guid OrganizationId
) : IRequest<OrganizationResponse>;

public record GetOrganizationsQuery
    : IRequest<IList<OrganizationResponse>>;

