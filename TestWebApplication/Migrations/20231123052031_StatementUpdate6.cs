using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class StatementUpdate6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_statement");

            migrationBuilder.AddColumn<int>(
                name: "StatementId",
                table: "Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Item_StatementId",
                table: "Item",
                column: "StatementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Statement_StatementId",
                table: "Item",
                column: "StatementId",
                principalTable: "Statement",
                principalColumn: "StatementId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Statement_StatementId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_StatementId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "StatementId",
                table: "Item");

            migrationBuilder.CreateTable(
                name: "item_statement",
                columns: table => new
                {
                    itemStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    statementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_statement", x => x.itemStateId);
                });
        }
    }
}
