-- TABLE: TICKET_ASSIGNMENTS - MAINTAINS THE ASSIGNMENT HISTORY OF AGENTS TO TICKETS, ALLOWING US TO TRACK WHO HANDLED A TICKET AND WHEN.

CREATE TABLE helpdesk.ticket_assignments
(
    assignment_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id      UUID NOT NULL,
    ticket_id            UUID NOT NULL,

    assigned_to_user_id  UUID NULL,
    assigned_by_user_id  UUID NOT NULL,

    assigned_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    unassigned_at        TIMESTAMPTZ NULL,

    created_at            TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_ticket_assignments_ticket
        FOREIGN KEY (organization_id, ticket_id)
        REFERENCES helpdesk.tickets
        (organization_id, ticket_id),

    CONSTRAINT fk_ticket_assignments_agent
        FOREIGN KEY (organization_id, assigned_to_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id),

    CONSTRAINT fk_ticket_assignments_assigned_by
        FOREIGN KEY (organization_id, assigned_by_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id)
);

CREATE INDEX idx_ticket_assignments_ticket
ON helpdesk.ticket_assignments
(organization_id, ticket_id);

CREATE INDEX idx_ticket_assignments_agent
ON helpdesk.ticket_assignments
(organization_id, assigned_to_user_id);

CREATE INDEX idx_ticket_assignments_history
ON helpdesk.ticket_assignments
(ticket_id, assigned_at DESC);


-- TABLE: TICKET_EVENTS - STORES THE COMPLETE AUDIT/HISTORY OF TICKET ACTIVITIES, SUCH AS STATUS CHANGES, ASSIGNMENTS, PRIORITY CHANGES, AND OTHER EVENTS.

CREATE TABLE helpdesk.ticket_events
(
    event_id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id      UUID NOT NULL,
    ticket_id            UUID NOT NULL,

    event_type           VARCHAR(100) NOT NULL,

    actor_user_id        UUID NULL,

    old_value            JSONB NULL,
    new_value            JSONB NULL,

    metadata             JSONB NULL,

    correlation_id       UUID NULL,

    occurred_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_ticket_events_ticket
        FOREIGN KEY (organization_id, ticket_id)
        REFERENCES helpdesk.tickets
        (organization_id, ticket_id),

    CONSTRAINT fk_ticket_events_actor
        FOREIGN KEY (organization_id, actor_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id)
);

CREATE INDEX idx_ticket_events_ticket_time
ON helpdesk.ticket_events
(
    organization_id,
    ticket_id,
    occurred_at DESC
);

CREATE INDEX idx_ticket_events_org_type
ON helpdesk.ticket_events
(
    organization_id,
    event_type
);

-- TABLE: TICKET_COMMENTS - STORES CONVERSATIONS AND NOTES ATTACHED TO TICKETS. SUPPORTS BOTH CUSTOMER-VISIBLE COMMENTS AND INTERNAL AGENT NOTES.

CREATE TABLE helpdesk.ticket_comments
(
    comment_id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id     UUID NOT NULL,
    ticket_id           UUID NOT NULL,

    author_user_id      UUID NOT NULL,

    comment_text        TEXT NOT NULL,

    is_internal         BOOLEAN NOT NULL DEFAULT FALSE,

    created_at           TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at           TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at           TIMESTAMPTZ NULL,

    CONSTRAINT fk_ticket_comments_ticket
        FOREIGN KEY (organization_id, ticket_id)
        REFERENCES helpdesk.tickets
        (organization_id, ticket_id),

    CONSTRAINT fk_ticket_comments_author
        FOREIGN KEY (organization_id, author_user_id)
        REFERENCES helpdesk.organization_users
        (organization_id, user_id)
);

CREATE INDEX idx_ticket_comments_ticket
ON helpdesk.ticket_comments
(organization_id, ticket_id, created_at ASC);

CREATE INDEX idx_ticket_comments_author
ON helpdesk.ticket_comments
(organization_id, author_user_id);