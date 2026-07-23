using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySpeech.Migrations
{
    /// <inheritdoc />
    public partial class BackfillTagOwners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tags predate per-user ownership and were shared globally, so the AddUserIdToTags
            // migration defaulted every row to "". Reassign each tag to the single user whose
            // notes reference it (tags used by more than one user, or by none, are left as "").
            migrationBuilder.Sql(@"
                UPDATE Tags
                SET UserId = (
                    SELECT n.UserId
                    FROM NoteTags nt
                    JOIN Notes n ON n.Id = nt.NoteId
                    WHERE nt.TagId = Tags.Id
                    GROUP BY nt.TagId
                    HAVING COUNT(DISTINCT n.UserId) = 1
                )
                WHERE UserId = ''
                AND EXISTS (
                    SELECT 1
                    FROM NoteTags nt
                    JOIN Notes n ON n.Id = nt.NoteId
                    WHERE nt.TagId = Tags.Id AND n.UserId <> ''
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not reversible: original ownership assignment is not recoverable.
        }
    }
}
