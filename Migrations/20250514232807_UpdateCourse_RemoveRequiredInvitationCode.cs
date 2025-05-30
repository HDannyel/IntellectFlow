using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellectFlow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourse_RemoveRequiredInvitationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_InvitationCode",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "InvitationCode",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitationCode",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InvitationCode",
                table: "Courses",
                column: "InvitationCode",
                unique: true);
        }
    }
}
