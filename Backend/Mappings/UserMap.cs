using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Table("users");
        Schema("helpdesk");

        Id(x => x.UserId)
            .Column("user_id")
            .GeneratedBy.Guid();

        Map(x => x.Email)
            .Column("email")
            .Length(320)
            .Not.Nullable()
            .Unique();

        Map(x => x.PasswordHash)
            .Column("password_hash")
            .Length(500)
            .Nullable();

        Map(x => x.FirstName)
            .Column("first_name")
            .Length(100)
            .Not.Nullable();

        Map(x => x.LastName)
            .Column("last_name")
            .Length(100)
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

        Map(x => x.DeletedAt)
            .Column("deleted_at")
            .Nullable();
    }
}