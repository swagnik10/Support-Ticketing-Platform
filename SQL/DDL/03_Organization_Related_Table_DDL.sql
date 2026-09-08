-- TABLE: TICKET_CATEGORIES - STORES TENANT-SPECIFIC CATEGORIES SUCH AS BUG, BILLING, TECHNICAL, ETC., USED TO CLASSIFY TICKETS.

CREATE TABLE helpdesk.ticket_categories
(
    category_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    organization_id  UUID NOT NULL,

    name             VARCHAR(100) NOT NULL,
    description      VARCHAR(500) NULL,

    is_active        BOOLEAN NOT NULL DEFAULT TRUE,

    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_ticket_categories_org_id
        UNIQUE (organization_id, category_id),

    CONSTRAINT uq_ticket_categories_name
        UNIQUE (organization_id, name),

    CONSTRAINT fk_ticket_categories_organization
        FOREIGN KEY (organization_id)
        REFERENCES helpdesk.organizations (organization_id)
);

-- TABLE: TICKET_PRIORITIES - DEFINES TICKET PRIORITY LEVELS SUCH AS LOW, MEDIUM, HIGH, AND CRITICAL. USED FOR PRIORITIZATION AND SLA DECISIONS.

CREATE TABLE helpdesk.ticket_priorities
(
    priority_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    organization_id  UUID NOT NULL,

    name              VARCHAR(50) NOT NULL,
    priority_level    INTEGER NOT NULL,

    is_active         BOOLEAN NOT NULL DEFAULT TRUE,

    created_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_ticket_priorities_org_id
        UNIQUE (organization_id, priority_id),

    CONSTRAINT uq_ticket_priorities_name
        UNIQUE (organization_id, name),

    CONSTRAINT uq_ticket_priorities_level
        UNIQUE (organization_id, priority_level),

    CONSTRAINT chk_ticket_priorities_level
        CHECK (priority_level > 0),

    CONSTRAINT fk_ticket_priorities_organization
        FOREIGN KEY (organization_id)
        REFERENCES helpdesk.organizations (organization_id)
);

-- TABLE: TICKET_STATUSES - DEFINES THE LIFECYCLE STATES OF A TICKET, SUCH AS OPEN, IN PROGRESS, RESOLVED, AND CLOSED.

CREATE TABLE helpdesk.ticket_statuses
(
    status_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    organization_id  UUID NOT NULL,

    name              VARCHAR(50) NOT NULL,
    status_code       VARCHAR(50) NOT NULL,

    is_initial        BOOLEAN NOT NULL DEFAULT FALSE,
    is_terminal       BOOLEAN NOT NULL DEFAULT FALSE,
    is_active         BOOLEAN NOT NULL DEFAULT TRUE,

    created_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_ticket_statuses_org_id
        UNIQUE (organization_id, status_id),

    CONSTRAINT uq_ticket_statuses_code
        UNIQUE (organization_id, status_code),

    CONSTRAINT fk_ticket_statuses_organization
        FOREIGN KEY (organization_id)
        REFERENCES helpdesk.organizations (organization_id)
);