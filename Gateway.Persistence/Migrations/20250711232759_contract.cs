using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class contract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyRoutePricing");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyRoutePricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    RouteId = table.Column<long>(type: "bigint", nullable: false),
                    BillingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxFreeCalls = table.Column<int>(type: "int", nullable: true),
                    MaxFreeCallsPerMonth = table.Column<int>(type: "int", nullable: true),
                    MonthlySubscriptionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PricePerCall = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TieredJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRoutePricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRoutePricing_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyRoutePricing_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoutePricing_CompanyId",
                table: "CompanyRoutePricing",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoutePricing_RouteId",
                table: "CompanyRoutePricing",
                column: "RouteId");
        }
    }
}
