INSERT INTO helpdesk.roles
(
    name,
    description,
    is_system_role
)
VALUES
(
    'Admin',
    'Organization administrator',
    TRUE
),
(
    'Agent',
    'Support agent handling tickets',
    TRUE
),
(
    'Customer',
    'Customer who creates and interacts with tickets',
    TRUE
)
ON CONFLICT (name) DO NOTHING;