using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySpeech.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSampleToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSample",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSample",
                table: "Notes");
        }
    }
}
