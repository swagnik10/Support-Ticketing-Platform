INSERT INTO helpdesk.organizations
(
    name,
    slug
)
VALUES
(
    'Demo Organization',
    'demo-organization'
)
ON CONFLICT (slug) DO NOTHING;