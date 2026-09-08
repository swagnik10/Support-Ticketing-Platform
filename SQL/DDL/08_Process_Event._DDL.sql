-- TABLE: PROCESSED_EVENTS - PROVIDES CONSUMER-SIDE IDEMPOTENCY BY RECORDING ALREADY-PROCESSED EVENTS AND PREVENTING DUPLICATE EVENT PROCESSING. 

CREATE TABLE helpdesk.processed_events
(
    processed_event_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    consumer_name      VARCHAR(200) NOT NULL,
    event_id            UUID NOT NULL,

    processed_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    metadata            JSONB NULL,

    CONSTRAINT uq_processed_events_consumer_event
        UNIQUE (consumer_name, event_id)
);

CREATE INDEX idx_processed_events_processed_at
ON helpdesk.processed_events
(processed_at);