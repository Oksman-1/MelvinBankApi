using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MelvinBankApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "76e92174-a577-4bfe-a819-18b98e31a082", "3e639688-c3ad-4222-b8ee-ad657937b3ac", "Administrator", "ADMINISTRATOR" },
                    { "c9c8f93f-6a74-4f16-b6ed-12fd4b2f6076", "0f2dff46-5562-4179-b6f5-97b7882d3cf4", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "76e92174-a577-4bfe-a819-18b98e31a082");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9c8f93f-6a74-4f16-b6ed-12fd4b2f6076");
        }
    }
}
