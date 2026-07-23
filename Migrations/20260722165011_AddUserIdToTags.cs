using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySpeech.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tags");
        }
    }
}
