-- TABLE: OUTBOX_MESSAGES - IMPLEMENTS THE TRANSACTIONAL OUTBOX PATTERN BY STORING EVENTS THAT MUST BE RELIABLY PUBLISHED TO RABBITMQ AFTER A DATABASE TRANSACTION. 

CREATE TABLE helpdesk.outbox_messages
(
    outbox_message_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    organization_id     UUID NULL,

    aggregate_type      VARCHAR(100) NOT NULL,
    aggregate_id        UUID NOT NULL,

    event_type          VARCHAR(100) NOT NULL,

    payload             JSONB NOT NULL,

    correlation_id      UUID NULL,

    occurred_at         TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    published_at        TIMESTAMPTZ NULL,

    retry_count         INTEGER NOT NULL DEFAULT 0,

    last_error          TEXT NULL,

    locked_at           TIMESTAMPTZ NULL,

    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT chk_outbox_retry_count
        CHECK (retry_count >= 0)
);

CREATE INDEX idx_outbox_unpublished
ON helpdesk.outbox_messages
(created_at)
WHERE published_at IS NULL;

CREATE INDEX idx_outbox_aggregate
ON helpdesk.outbox_messages
(aggregate_type, aggregate_id);