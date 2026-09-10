using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketSlaMap : ClassMap<TicketSla>
{
    public TicketSlaMap()
    {
        Table("ticket_sla");
        Schema("helpdesk");

        Id(x => x.TicketSlaId)
            .Column("ticket_sla_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.TicketId)
            .Column("ticket_id")
            .Not.Nullable();

        Map(x => x.SlaPolicyId)
            .Column("sla_policy_id")
            .Not.Nullable();

        Map(x => x.FirstResponseDueAt)
            .Column("first_response_due_at")
            .Nullable();

        Map(x => x.FirstRespondedAt)
            .Column("first_responded_at")
            .Nullable();

        Map(x => x.ResolutionDueAt)
            .Column("resolution_due_at")
            .Nullable();

        Map(x => x.ResolvedAt)
            .Column("resolved_at")
            .Nullable();

        Map(x => x.FirstResponseBreached)
            .Column("first_response_breached")
            .Not.Nullable();

        Map(x => x.ResolutionBreached)
            .Column("resolution_breached")
            .Not.Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();

        Map(x => x.UpdatedAt)
            .Column("updated_at")
            .Not.Nullable();
    }
}