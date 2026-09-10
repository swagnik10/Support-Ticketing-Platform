using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketAssignmentMap : ClassMap<TicketAssignment>
{
    public TicketAssignmentMap()
    {
        Table("ticket_assignments");
        Schema("helpdesk");

        Id(x => x.AssignmentId)
            .Column("assignment_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.TicketId)
            .Column("ticket_id")
            .Not.Nullable();

        Map(x => x.AssignedToUserId)
            .Column("assigned_to_user_id")
            .Nullable();

        Map(x => x.AssignedByUserId)
            .Column("assigned_by_user_id")
            .Not.Nullable();

        Map(x => x.AssignedAt)
            .Column("assigned_at")
            .Not.Nullable();

        Map(x => x.UnassignedAt)
            .Column("unassigned_at")
            .Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();
    }
}