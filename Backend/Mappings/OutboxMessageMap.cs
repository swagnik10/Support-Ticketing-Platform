using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class OutboxMessageMap : ClassMap<OutboxMessage>
{
    public OutboxMessageMap()
    {
        Table("outbox_messages");
        Schema("helpdesk");

        Id(x => x.OutboxMessageId)
            .Column("outbox_message_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Nullable();

        Map(x => x.AggregateType)
            .Column("aggregate_type")
            .Length(100)
            .Not.Nullable();

        Map(x => x.AggregateId)
            .Column("aggregate_id")
            .Not.Nullable();

        Map(x => x.EventType)
            .Column("event_type")
            .Length(100)
            .Not.Nullable();

        Map(x => x.Payload)
            .Column("payload")
            .CustomSqlType("jsonb")
            .Not.Nullable();

        Map(x => x.CorrelationId)
            .Column("correlation_id")
            .Nullable();

        Map(x => x.OccurredAt)
            .Column("occurred_at")
            .Not.Nullable();

        Map(x => x.PublishedAt)
            .Column("published_at")
            .Nullable();

        Map(x => x.RetryCount)
            .Column("retry_count")
            .Not.Nullable();

        Map(x => x.LastError)
            .Column("last_error")
            .CustomSqlType("text")
            .Nullable();

        Map(x => x.LockedAt)
            .Column("locked_at")
            .Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();
    }
}