using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEnumDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tasks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Todo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tasks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Todo",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
