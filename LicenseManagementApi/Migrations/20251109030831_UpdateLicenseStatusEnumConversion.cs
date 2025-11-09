using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLicenseStatusEnumConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Licenses_Skus_SkuId",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_SkuId",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "SkuId",
                table: "Licenses");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Licenses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "LicenseSkus",
                columns: table => new
                {
                    LicenseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseSkus", x => new { x.LicenseId, x.SkuId });
                    table.ForeignKey(
                        name: "FK_LicenseSkus_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseSkus_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicenseSkus_LicenseId",
                table: "LicenseSkus",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseSkus_SkuId",
                table: "LicenseSkus",
                column: "SkuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseSkus");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Licenses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "SkuId",
                table: "Licenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_SkuId",
                table: "Licenses",
                column: "SkuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Licenses_Skus_SkuId",
                table: "Licenses",
                column: "SkuId",
                principalTable: "Skus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
