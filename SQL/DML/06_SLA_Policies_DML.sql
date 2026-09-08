INSERT INTO helpdesk.sla_policies
(
    organization_id,
    name,
    description,
    first_response_minutes,
    resolution_minutes,
    is_default
)
SELECT
    organization_id,
    'Standard SLA',
    'Default SLA policy for support tickets',
    60,
    1440,
    TRUE
FROM helpdesk.organizations
WHERE slug = 'demo-organization'
ON CONFLICT (organization_id, name) DO NOTHING;