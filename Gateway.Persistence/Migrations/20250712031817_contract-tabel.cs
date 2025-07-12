using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class contracttabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPlans_Companies_CompanyId",
                table: "CompanyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPlans_Plans_PlanId",
                table: "CompanyPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyPlans",
                table: "CompanyPlans");

            migrationBuilder.RenameTable(
                name: "CompanyPlans",
                newName: "Contracts");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPlans_PlanId",
                table: "Contracts",
                newName: "IX_Contracts_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyPlans_CompanyId",
                table: "Contracts",
                newName: "IX_Contracts_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Plans_PlanId",
                table: "Contracts",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Plans_PlanId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "CompanyPlans");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_PlanId",
                table: "CompanyPlans",
                newName: "IX_CompanyPlans_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_CompanyId",
                table: "CompanyPlans",
                newName: "IX_CompanyPlans_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyPlans",
                table: "CompanyPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPlans_Companies_CompanyId",
                table: "CompanyPlans",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPlans_Plans_PlanId",
                table: "CompanyPlans",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
