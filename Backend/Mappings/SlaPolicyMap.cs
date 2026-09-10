using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class SlaPolicyMap : ClassMap<SlaPolicy>
{
    public SlaPolicyMap()
    {
        Table("sla_policies");
        Schema("helpdesk");

        Id(x => x.SlaPolicyId)
            .Column("sla_policy_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.Name)
            .Column("name")
            .Length(150)
            .Not.Nullable();

        Map(x => x.Description)
            .Column("description")
            .Length(500)
            .Nullable();

        Map(x => x.PriorityId)
            .Column("priority_id")
            .Nullable();

        Map(x => x.FirstResponseMinutes)
            .Column("first_response_minutes")
            .Not.Nullable();

        Map(x => x.ResolutionMinutes)
            .Column("resolution_minutes")
            .Not.Nullable();

        Map(x => x.IsDefault)
            .Column("is_default")
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