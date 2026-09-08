INSERT INTO helpdesk.ticket_statuses
(
    organization_id,
    name,
    status_code,
    is_initial,
    is_terminal
)
SELECT
    organization_id,
    status.name,
    status.status_code,
    status.is_initial,
    status.is_terminal
FROM helpdesk.organizations o
CROSS JOIN
(
    VALUES
        ('Open', 'OPEN', TRUE, FALSE),
        ('In Progress', 'IN_PROGRESS', FALSE, FALSE),
        ('Waiting for Customer', 'WAITING_FOR_CUSTOMER', FALSE, FALSE),
        ('Resolved', 'RESOLVED', FALSE, TRUE),
        ('Closed', 'CLOSED', FALSE, TRUE)
) AS status
(
    name,
    status_code,
    is_initial,
    is_terminal
)
WHERE o.slug = 'demo-organization'
ON CONFLICT (organization_id, status_code) DO NOTHING;