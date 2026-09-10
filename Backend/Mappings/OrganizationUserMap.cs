using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class OrganizationUserMap : ClassMap<OrganizationUser>
{
    public OrganizationUserMap()
    {
        Table("organization_users");
        Schema("helpdesk");

        CompositeId()
            .KeyProperty(x => x.OrganizationId, "organization_id")
            .KeyProperty(x => x.UserId, "user_id");

        Map(x => x.RoleId)
            .Column("role_id")
            .Not.Nullable();

        Map(x => x.IsActive)
            .Column("is_active")
            .Not.Nullable();

        Map(x => x.JoinedAt)
            .Column("joined_at")
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