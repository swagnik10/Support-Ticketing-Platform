-- TABLE: ORGANIZATIONS - REPRESENTS EACH TENANT/COMPANY USING THE SAAS PLATFORM. IT IS THE ROOT OF TENANT ISOLATION AND OWNERSHIP OF TENANT-SPECIFIC DATA.

CREATE TABLE helpdesk.organizations
(
    organization_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    name            VARCHAR(200) NOT NULL,
    slug            VARCHAR(100) NOT NULL,

    is_active       BOOLEAN NOT NULL DEFAULT TRUE,

    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at      TIMESTAMPTZ NULL,

    CONSTRAINT uq_organizations_slug
        UNIQUE (slug)
);

CREATE INDEX idx_organizations_active
ON helpdesk.organizations (is_active);

