-- TABLE: TICKETS - THE CORE BUSINESS ENTITY REPRESENTING CUSTOMER SUPPORT REQUESTS. CONNECTS THE CUSTOMER, AGENT, CATEGORY, PRIORITY, AND STATUS.

CREATE TABLE helpdesk.tickets
(
    ticket_id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id       UUID NOT NULL,

    ticket_number         BIGINT GENERATED ALWAYS AS IDENTITY,

    subject               VARCHAR(300) NOT NULL,
    description           TEXT NOT NULL,

    customer_user_id      UUID NOT NULL,
    assigned_agent_id     UUID NULL,

    category_id           UUID NOT NULL,
    priority_id           UUID NOT NULL,
    status_id             UUID NOT NULL,

    created_by_user_id    UUID NOT NULL,

    created_at            TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at            TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    resolved_at           TIMESTAMPTZ NULL,
    closed_at             TIMESTAMPTZ NULL,
    deleted_at            TIMESTAMPTZ NULL,

    CONSTRAINT uq_tickets_org_ticket
        UNIQUE (organization_id, ticket_id),

    CONSTRAINT uq_tickets_ticket_number
        UNIQUE (organization_id, ticket_number),

    -- Customer must belong to same organization
    CONSTRAINT fk_tickets_customer
        FOREIGN KEY (organization_id, customer_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id),

    -- Assigned agent must belong to same organization
    CONSTRAINT fk_tickets_assigned_agent
        FOREIGN KEY (organization_id, assigned_agent_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id),

    -- Ticket creator must belong to same organization
    CONSTRAINT fk_tickets_created_by
        FOREIGN KEY (organization_id, created_by_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id),

    -- Category must belong to same organization
    CONSTRAINT fk_tickets_category
        FOREIGN KEY (organization_id, category_id)
        REFERENCES helpdesk.ticket_categories
        (organization_id, category_id),

    -- Priority must belong to same organization
    CONSTRAINT fk_tickets_priority
        FOREIGN KEY (organization_id, priority_id)
        REFERENCES helpdesk.ticket_priorities
        (organization_id, priority_id),

    -- Status must belong to same organization
    CONSTRAINT fk_tickets_status
        FOREIGN KEY (organization_id, status_id)
        REFERENCES helpdesk.ticket_statuses
        (organization_id, status_id)
);

CREATE INDEX idx_tickets_org_status
ON helpdesk.tickets
(
    organization_id,
    status_id
);

CREATE INDEX idx_tickets_org_agent_status
ON helpdesk.tickets
(
    organization_id,
    assigned_agent_id,
    status_id
);

CREATE INDEX idx_tickets_org_created
ON helpdesk.tickets
(
    organization_id,
    created_at DESC
);