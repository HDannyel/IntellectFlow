using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Teachers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Students",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MidleName",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MidleName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "MidleName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MidleName",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Teachers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Students",
                newName: "UserId");
        }
    }
}
