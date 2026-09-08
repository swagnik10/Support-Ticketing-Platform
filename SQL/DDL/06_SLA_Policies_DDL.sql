-- TABLE: SLA_POLICIES - DEFINES ORGANIZATION-SPECIFIC SLA RULES, SUCH AS FIRST-RESPONSE AND RESOLUTION TARGETS BASED ON TICKET PRIORITY.

CREATE TABLE helpdesk.sla_policies
(
    sla_policy_id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id        UUID NOT NULL,

    name                   VARCHAR(150) NOT NULL,
    description            VARCHAR(500) NULL,

    priority_id            UUID NULL,

    first_response_minutes INTEGER NOT NULL,
    resolution_minutes     INTEGER NOT NULL,

    is_default             BOOLEAN NOT NULL DEFAULT FALSE,
    is_active              BOOLEAN NOT NULL DEFAULT TRUE,

    created_at             TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at             TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_sla_policies_org_id
        UNIQUE (organization_id, sla_policy_id),

    CONSTRAINT uq_sla_policies_name
        UNIQUE (organization_id, name),

    CONSTRAINT chk_sla_first_response
        CHECK (first_response_minutes > 0),

    CONSTRAINT chk_sla_resolution
        CHECK (resolution_minutes > 0),

    CONSTRAINT fk_sla_policies_organization
        FOREIGN KEY (organization_id)
        REFERENCES helpdesk.organizations
        (organization_id),

    CONSTRAINT fk_sla_policies_priority
        FOREIGN KEY (organization_id, priority_id)
        REFERENCES helpdesk.ticket_priorities
        (organization_id, priority_id)
);

CREATE INDEX idx_sla_policies_organization
ON helpdesk.sla_policies
(organization_id);

-- TABLE: TICKET_SLA - TRACKS THE SLA STATE AND DEADLINES FOR AN INDIVIDUAL TICKET, INCLUDING RESPONSE/RESOLUTION TIMES AND BREACHES.

CREATE TABLE helpdesk.ticket_sla
(
    ticket_sla_id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id        UUID NOT NULL,
    ticket_id              UUID NOT NULL,
    sla_policy_id          UUID NOT NULL,

    first_response_due_at  TIMESTAMPTZ NULL,
    first_responded_at     TIMESTAMPTZ NULL,

    resolution_due_at     TIMESTAMPTZ NULL,
    resolved_at            TIMESTAMPTZ NULL,

    first_response_breached BOOLEAN NOT NULL DEFAULT FALSE,
    resolution_breached     BOOLEAN NOT NULL DEFAULT FALSE,

    created_at             TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at             TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_ticket_sla_ticket
        UNIQUE (organization_id, ticket_id),

    CONSTRAINT fk_ticket_sla_ticket
        FOREIGN KEY (organization_id, ticket_id)
        REFERENCES helpdesk.tickets
        (organization_id, ticket_id),

    CONSTRAINT fk_ticket_sla_policy
        FOREIGN KEY (organization_id, sla_policy_id)
        REFERENCES helpdesk.sla_policies
        (organization_id, sla_policy_id)
);

CREATE INDEX idx_ticket_sla_due_resolution
ON helpdesk.ticket_sla
(organization_id, resolution_due_at);

CREATE INDEX idx_ticket_sla_first_response
ON helpdesk.ticket_sla
(organization_id, first_response_due_at);