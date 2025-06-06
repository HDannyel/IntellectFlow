using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntellectFlow.Migrations
{
    /// <inheritdoc />
    public partial class NewEntitiesToStudentTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "StudentTaskSubmissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTaskSubmissions_DocumentId",
                table: "StudentTaskSubmissions",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTaskSubmissions_Documents_DocumentId",
                table: "StudentTaskSubmissions",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTaskSubmissions_Documents_DocumentId",
                table: "StudentTaskSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentTaskSubmissions_DocumentId",
                table: "StudentTaskSubmissions");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "StudentTaskSubmissions");
        }
    }
}
