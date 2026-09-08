-- TABLE: ROLES - DEFINES ACCESS ROLES SUCH AS ADMIN, AGENT, AND CUSTOMER FOR AUTHORIZATION/RBAC.

CREATE TABLE helpdesk.roles
(
    role_id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    name            VARCHAR(50) NOT NULL,
    description     VARCHAR(250) NULL,

    is_system_role  BOOLEAN NOT NULL DEFAULT FALSE,

    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_roles_name
        UNIQUE (name)
);

-- TABLE: USERS - STORES THE PLATFORM'S USER IDENTITIES, INCLUDING CUSTOMERS, AGENTS, AND ADMINISTRATORS.

CREATE TABLE helpdesk.users
(
    user_id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    email           VARCHAR(320) NOT NULL,
    password_hash   VARCHAR(500) NULL,

    first_name      VARCHAR(100) NOT NULL,
    last_name       VARCHAR(100) NULL,

    is_active       BOOLEAN NOT NULL DEFAULT TRUE,

    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ NULL,

    CONSTRAINT uq_users_email
        UNIQUE (email)
);

CREATE INDEX idx_users_email
ON helpdesk.users (email);

CREATE INDEX idx_users_active
ON helpdesk.users (is_active);


-- TABLE: ORGANIZATION_USERS - MAPS USERS TO ORGANIZATIONS AND ASSIGNS THEIR ROLE WITHIN EACH ORGANIZATION. THIS IS THE KEY MEMBERSHIP/TENANT RELATIONSHIP.

CREATE TABLE helpdesk.organization_users
(
    organization_id UUID NOT NULL,
    user_id         UUID NOT NULL,
    role_id         UUID NOT NULL,

    is_active       BOOLEAN NOT NULL DEFAULT TRUE,

    joined_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ NULL,

    PRIMARY KEY (organization_id, user_id),

    CONSTRAINT fk_org_users_organization
        FOREIGN KEY (organization_id)
        REFERENCES helpdesk.organizations (organization_id),

    CONSTRAINT fk_org_users_user
        FOREIGN KEY (user_id)
        REFERENCES helpdesk.users (user_id),

    CONSTRAINT fk_org_users_role
        FOREIGN KEY (role_id)
        REFERENCES helpdesk.roles (role_id)
);

CREATE INDEX idx_org_users_user
ON helpdesk.organization_users (user_id);

CREATE INDEX idx_org_users_role
ON helpdesk.organization_users (organization_id, role_id);

CREATE INDEX idx_org_users_active
ON helpdesk.organization_users (organization_id, is_active);
