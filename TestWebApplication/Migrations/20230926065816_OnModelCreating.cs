using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWebApplication.Migrations
{
    public partial class OnModelCreating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c44b011b-a25a-436b-bd87-ba600c942df3", 0, "561ad666-24fe-4ba1-9db6-e3ee4c03dc43", "superadmin@gmail.com", true, true, null, "SUPERADMIN@GMAIL.COM", "SUPERADMIN@GMAIL.COM", "AQAAAAEAACcQAAAAEKou4krBVEylwJ8+x1rw/Rfiq+YL/w2kE4NJ6+8WI0+gMhmDh2Vosu0wLVZ53NzZew==", null, false, "b1804040-602f-4d2b-8794-269fdacf7874", false, "superadmin@gmail.com" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "CreatedBy", "CreatedOn", "IsActive", "IsInternal", "ModifiedBy", "ModifiedOn", "RoleDesc", "RoleType" },
                values: new object[] { new Guid("15391037-debe-4dab-b8b8-4cd14e1efe3b"), "System", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local), true, true, "System", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local), "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleID", "CreatedBy", "CreatedOn", "IsActive", "IsInternal", "ModifiedBy", "ModifiedOn", "RoleDesc", "RoleType" },
                values: new object[] { new Guid("363432dc-f55d-43bd-9f8c-8f764817319e"), "System", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local), true, true, "System", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local), "SuperAdmin", "SUPERADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "363432dc-f55d-43bd-9f8c-8f764817319e", "c44b011b-a25a-436b-bd87-ba600c942df3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "363432dc-f55d-43bd-9f8c-8f764817319e", "c44b011b-a25a-436b-bd87-ba600c942df3" });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: new Guid("15391037-debe-4dab-b8b8-4cd14e1efe3b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleID",
                keyValue: new Guid("363432dc-f55d-43bd-9f8c-8f764817319e"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c44b011b-a25a-436b-bd87-ba600c942df3");
        }
    }
}
