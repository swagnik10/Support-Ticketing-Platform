using Backend.DTO.SlaPolicy;
using MediatR;

namespace Backend.CommandAndQuery;

public record CreateSlaPolicyCommand(
    CreateSlaPolicyRequest Request
) : IRequest<SlaPolicyResponse>;

public record GetSlaPoliciesQuery(
    Guid OrganizationId
) : IRequest<IList<SlaPolicyResponse>>;

public record GetSlaPolicyQuery(
    Guid SlaPolicyId,
    Guid OrganizationId
) : IRequest<SlaPolicyResponse>;

public record UpdateSlaPolicyCommand(
    Guid SlaPolicyId,
    UpdateSlaPolicyRequest Request
) : IRequest<SlaPolicyResponse>;

public record DeactivateSlaPolicyCommand(
    Guid SlaPolicyId,
    Guid OrganizationId
) : IRequest<bool>;