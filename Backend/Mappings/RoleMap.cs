using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class RoleMap : ClassMap<Role>
{
    public RoleMap()
    {
        Table("roles");
        Schema("helpdesk");

        Id(x => x.RoleId)
            .Column("role_id")
            .GeneratedBy.Guid();

        Map(x => x.Name)
            .Column("name")
            .Length(50)
            .Not.Nullable();

        Map(x => x.Description)
            .Column("description")
            .Length(250)
            .Nullable();

        Map(x => x.IsSystemRole)
            .Column("is_system_role")
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();
    }
}