using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class AlternateColumnAdding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltContact",
                table: "Incident",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltEmail",
                table: "Incident",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltPhone",
                table: "Incident",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltContact",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "AltEmail",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "AltPhone",
                table: "Incident");
        }
    }
}
