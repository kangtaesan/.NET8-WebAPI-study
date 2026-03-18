using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class CommentOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "249bc4b3-22b8-474e-9d67-2657c6437128");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3b68cc8-1724-4472-ad87-a1edd23a15af");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Stocks",
                newName: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "68b2aa30-3b7e-4803-902c-a83845941164", null, "User", "USER" },
                    { "e417f751-9b6d-4d5c-b261-7a72980b2f7e", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68b2aa30-3b7e-4803-902c-a83845941164");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e417f751-9b6d-4d5c-b261-7a72980b2f7e");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Stocks",
                newName: "id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "249bc4b3-22b8-474e-9d67-2657c6437128", null, "Admin", "ADMIN" },
                    { "f3b68cc8-1724-4472-ad87-a1edd23a15af", null, "User", "USER" }
                });
        }
    }
}
