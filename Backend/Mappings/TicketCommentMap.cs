using Backend.Domain;
using FluentNHibernate.Mapping;

namespace Backend.Mappings;

public class TicketCommentMap : ClassMap<TicketComment>
{
    public TicketCommentMap()
    {
        Table("ticket_comments");
        Schema("helpdesk");

        Id(x => x.CommentId)
            .Column("comment_id")
            .GeneratedBy.Guid();

        Map(x => x.OrganizationId)
            .Column("organization_id")
            .Not.Nullable();

        Map(x => x.TicketId)
            .Column("ticket_id")
            .Not.Nullable();

        Map(x => x.AuthorUserId)
            .Column("author_user_id")
            .Not.Nullable();

        Map(x => x.CommentText)
            .Column("comment_text")
            .CustomSqlType("text")
            .Not.Nullable();

        Map(x => x.IsInternal)
            .Column("is_internal")
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