using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class usagelogkeyIdlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "KeyId",
                table: "UsageLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyId",
                table: "UsageLogs");
        }
    }
}
