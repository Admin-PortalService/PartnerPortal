using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class SLA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Severity",
                newName: "PrjStatus");

            migrationBuilder.AddColumn<string>(
                name: "IncidentStatus",
                table: "Severity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBreached",
                table: "Incident",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PauseDuration",
                table: "Incident",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PauseEnd",
                table: "Incident",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PauseStart",
                table: "Incident",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SLADate",
                table: "Incident",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncidentHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SLAConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusID = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLAName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLAType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLAmin = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLAConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentHistory");

            migrationBuilder.DropTable(
                name: "SLAConfig");

            migrationBuilder.DropColumn(
                name: "IncidentStatus",
                table: "Severity");

            migrationBuilder.DropColumn(
                name: "HasBreached",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "PauseDuration",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "PauseEnd",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "PauseStart",
                table: "Incident");

            migrationBuilder.DropColumn(
                name: "SLADate",
                table: "Incident");

            migrationBuilder.RenameColumn(
                name: "PrjStatus",
                table: "Severity",
                newName: "Status");
        }
    }
}
