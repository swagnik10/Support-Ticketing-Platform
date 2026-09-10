using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketEventMap : ClassMap<TicketEvent>
{
    public TicketEventMap()
    {
        Table("ticket_events");
        Schema("helpdesk");

        Id(x => x.EventId)
            .Column("event_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.TicketId)
            .Column("ticket_id")
            .Not.Nullable();

        Map(x => x.EventType)
            .Column("event_type")
            .Length(100)
            .Not.Nullable();

        Map(x => x.ActorUserId)
            .Column("actor_user_id")
            .Nullable();

        Map(x => x.OldValue)
            .Column("old_value")
            .CustomSqlType("jsonb")
            .Nullable();

        Map(x => x.NewValue)
            .Column("new_value")
            .CustomSqlType("jsonb")
            .Nullable();

        Map(x => x.Metadata)
            .Column("metadata")
            .CustomSqlType("jsonb")
            .Nullable();

        Map(x => x.CorrelationId)
            .Column("correlation_id")
            .Nullable();

        Map(x => x.OccurredAt)
            .Column("occurred_at")
            .Not.Nullable();
    }
}