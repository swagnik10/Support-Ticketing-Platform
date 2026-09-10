using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketStatusMap : ClassMap<TicketStatus>
{
    public TicketStatusMap()
    {
        Table("ticket_statuses");
        Schema("helpdesk");

        Id(x => x.StatusId)
            .Column("status_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.Name)
            .Column("name")
            .Length(50)
            .Not.Nullable();

        Map(x => x.StatusCode)
            .Column("status_code")
            .Length(50)
            .Not.Nullable();

        Map(x => x.IsInitial)
            .Column("is_initial")
            .Not.Nullable();

        Map(x => x.IsTerminal)
            .Column("is_terminal")
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