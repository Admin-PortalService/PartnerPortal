using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class StatementUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Statement");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Statement",
                newName: "ItemDes");

            migrationBuilder.AddColumn<string>(
                name: "DocDes",
                table: "Statement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocDes",
                table: "Statement");

            migrationBuilder.RenameColumn(
                name: "ItemDes",
                table: "Statement",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "TotalAmount",
                table: "Statement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
