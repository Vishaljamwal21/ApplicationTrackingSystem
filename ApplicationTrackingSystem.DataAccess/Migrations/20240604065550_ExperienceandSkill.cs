using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTrackingSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ExperienceandSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "applyjobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "applyjobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "applyjobs");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "applyjobs");
        }
    }
}
