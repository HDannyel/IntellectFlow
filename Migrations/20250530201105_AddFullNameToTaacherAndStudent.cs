using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameToTaacherAndStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MidleName",
                table: "Teachers",
                newName: "MiddleName");

            migrationBuilder.RenameColumn(
                name: "MidleName",
                table: "Students",
                newName: "MiddleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "Teachers",
                newName: "MidleName");

            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "Students",
                newName: "MidleName");
        }
    }
}
