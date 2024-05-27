using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTrackingSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class applyjobModel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobPostId",
                table: "applyjobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_applyjobs_JobPostId",
                table: "applyjobs",
                column: "JobPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_applyjobs_JobPosts_JobPostId",
                table: "applyjobs",
                column: "JobPostId",
                principalTable: "JobPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applyjobs_JobPosts_JobPostId",
                table: "applyjobs");

            migrationBuilder.DropIndex(
                name: "IX_applyjobs_JobPostId",
                table: "applyjobs");

            migrationBuilder.DropColumn(
                name: "JobPostId",
                table: "applyjobs");
        }
    }
}
