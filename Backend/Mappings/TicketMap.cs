using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketMap : ClassMap<Ticket>
{
    public TicketMap()
    {
        Table("tickets");
        Schema("helpdesk");

        Id(x => x.TicketId)
            .Column("ticket_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.TicketNumber)
            .Column("ticket_number")
            .Generated.Always();
        
        Map(x => x.Subject)
            .Column("subject")
            .Length(300)
            .Not.Nullable();

        Map(x => x.Description)
            .Column("description")
            .CustomSqlType("text")
            .Not.Nullable();

        Map(x => x.CustomerUserId)
            .Column("customer_user_id")
            .Not.Nullable();

        Map(x => x.AssignedAgentId)
            .Column("assigned_agent_id")
            .Nullable();

        Map(x => x.CategoryId)
            .Column("category_id")
            .Not.Nullable();

        Map(x => x.PriorityId)
            .Column("priority_id")
            .Not.Nullable();

        Map(x => x.StatusId)
            .Column("status_id")
            .Not.Nullable();

        Map(x => x.CreatedByUserId)
            .Column("created_by_user_id")
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();

        Map(x => x.UpdatedAt)
            .Column("updated_at")
            .Not.Nullable();

        Map(x => x.ResolvedAt)
            .Column("resolved_at")
            .Nullable();

        Map(x => x.ClosedAt)
            .Column("closed_at")
            .Nullable();

        Map(x => x.DeletedAt)
            .Column("deleted_at")
            .Nullable();
    }
}