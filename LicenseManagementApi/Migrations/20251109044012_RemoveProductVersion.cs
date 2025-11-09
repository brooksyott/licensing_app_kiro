using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
