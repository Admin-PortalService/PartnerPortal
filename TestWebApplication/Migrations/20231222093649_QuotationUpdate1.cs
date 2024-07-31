using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class QuotationUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quotation",
                columns: table => new
                {
                    QuotationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organization = table.Column<int>(type: "int", nullable: false),
                    Project = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotation", x => x.QuotationId);
                });

            migrationBuilder.CreateTable(
                name: "ItemQuotation",
                columns: table => new
                {
                    itemQuotationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quotationId = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemQuotation", x => x.itemQuotationId);
                    table.ForeignKey(
                        name: "FK_ItemQuotation_Item_itemId",
                        column: x => x.itemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemQuotation_Quotation_quotationId",
                        column: x => x.quotationId,
                        principalTable: "Quotation",
                        principalColumn: "QuotationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemQuotation_itemId",
                table: "ItemQuotation",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemQuotation_quotationId",
                table: "ItemQuotation",
                column: "quotationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemQuotation");

            migrationBuilder.DropTable(
                name: "Quotation");
        }
    }
}
