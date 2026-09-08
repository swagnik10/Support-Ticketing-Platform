INSERT INTO helpdesk.ticket_priorities
(
    organization_id,
    name,
    priority_level
)
SELECT
    organization_id,
    priority.name,
    priority.priority_level
FROM helpdesk.organizations o
CROSS JOIN
(
    VALUES
        ('Low', 1),
        ('Medium', 2),
        ('High', 3),
        ('Critical', 4)
) AS priority(name, priority_level)
WHERE o.slug = 'demo-organization'
ON CONFLICT (organization_id, name) DO NOTHING;