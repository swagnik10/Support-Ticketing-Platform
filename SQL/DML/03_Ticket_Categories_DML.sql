INSERT INTO helpdesk.ticket_categories
(
    organization_id,
    name,
    description
)
SELECT
    organization_id,
    category.name,
    category.description
FROM helpdesk.organizations o
CROSS JOIN
(
    VALUES
        ('Bug', 'Software defect or unexpected behavior'),
        ('Billing', 'Billing and payment related issue'),
        ('Technical', 'Technical support issue'),
        ('Account', 'Account related issue'),
        ('General', 'General support request')
) AS category(name, description)
WHERE o.slug = 'demo-organization'
ON CONFLICT (organization_id, name) DO NOTHING;