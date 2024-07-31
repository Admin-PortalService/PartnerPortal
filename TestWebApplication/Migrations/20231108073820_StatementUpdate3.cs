using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class StatementUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Statement",
                newName: "StatementId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Item",
                newName: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatementId",
                table: "Statement",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Item",
                newName: "Id");
        }
    }
}
