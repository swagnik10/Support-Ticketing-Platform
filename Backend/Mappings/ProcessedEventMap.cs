using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class ProcessedEventMap : ClassMap<ProcessedEvent>
{
    public ProcessedEventMap()
    {
        Table("processed_events");
        Schema("helpdesk");

        Id(x => x.ProcessedEventId)
            .Column("processed_event_id")
            .GeneratedBy.Guid();

        Map(x => x.ConsumerName)
            .Column("consumer_name")
            .Length(200)
            .Not.Nullable();

        Map(x => x.EventId)
            .Column("event_id")
            .Not.Nullable();

        Map(x => x.ProcessedAt)
            .Column("processed_at")
            .Not.Nullable();

        Map(x => x.Metadata)
            .Column("metadata")
            .CustomSqlType("jsonb")
            .Nullable();
    }
}