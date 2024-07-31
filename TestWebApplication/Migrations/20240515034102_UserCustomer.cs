using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class UserCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "UserAccess");

            migrationBuilder.CreateTable(
                name: "User_Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Customer_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Customer_UserAccess_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccess",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Customer_CustomerID",
                table: "User_Customer",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Customer_UserID",
                table: "User_Customer",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Customer");
                        
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "UserAccess",
                type: "int",
                nullable: true);
        }
    }
}
