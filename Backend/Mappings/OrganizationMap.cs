using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class OrganizationMap : ClassMap<Organization>
{
    public OrganizationMap()
    {
        Table("organizations");
        Schema("helpdesk");

        Id(x => x.OrganizationId)
            .Column("organization_id")
            .GeneratedBy.Guid();

        Map(x => x.Name)
            .Column("name")
            .Length(200)
            .Not.Nullable();

        Map(x => x.Slug)
            .Column("slug")
            .Length(100)
            .Not.Nullable()
            .Unique();

        Map(x => x.IsActive)
            .Column("is_active")
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();

        Map(x => x.UpdatedAt)
            .Column("updated_at")
            .Not.Nullable();

        Map(x => x.DeletedAt)
            .Column("deleted_at")
            .Nullable();
    }
}