using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketCategoryMap : ClassMap<TicketCategory>
{
    public TicketCategoryMap()
    {
        Table("ticket_categories");
        Schema("helpdesk");

        Id(x => x.CategoryId)
            .Column("category_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.Name)
            .Column("name")
            .Length(100)
            .Not.Nullable();

        Map(x => x.Description)
            .Column("description")
            .Length(500)
            .Nullable();

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