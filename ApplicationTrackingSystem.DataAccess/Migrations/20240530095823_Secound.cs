using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTrackingSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Secound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "formLinks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "formLinks");
        }
    }
}
