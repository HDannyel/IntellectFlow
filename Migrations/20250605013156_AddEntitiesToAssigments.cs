using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesToAssigments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_DocumentId",
                table: "Assignments",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Documents_DocumentId",
                table: "Assignments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Documents_DocumentId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_DocumentId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Assignments");
        }
    }
}
