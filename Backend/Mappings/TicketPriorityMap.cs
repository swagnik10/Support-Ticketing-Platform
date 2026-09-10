using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketPriorityMap : ClassMap<TicketPriority>
{
    public TicketPriorityMap()
    {
        Table("ticket_priorities");
        Schema("helpdesk");

        Id(x => x.PriorityId)
            .Column("priority_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.Name)
            .Column("name")
            .Length(50)
            .Not.Nullable();

        Map(x => x.PriorityLevel)
            .Column("priority_level")
            .Not.Nullable();

        Map(x => x.IsActive)
            .Column("is_active")
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();

        Map(x => x.UpdatedAt)
            .Column("updated_at")
            .Not.Nullable();
    }
}