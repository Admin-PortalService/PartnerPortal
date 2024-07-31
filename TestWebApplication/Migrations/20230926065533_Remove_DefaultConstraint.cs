using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class Remove_DefaultConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", "AspNetUserRoles");
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        table: "AspNetUserRoles",
                        column: "RoleId",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
        }
    }
}
