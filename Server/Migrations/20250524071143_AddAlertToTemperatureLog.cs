using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertToTemperatureLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Alert",
                table: "TemperatureLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AlertMessage",
                table: "TemperatureLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TemperatureLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Alert",
                table: "SmokeLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AlertMessage",
                table: "SmokeLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alert",
                table: "TemperatureLogs");

            migrationBuilder.DropColumn(
                name: "AlertMessage",
                table: "TemperatureLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TemperatureLogs");

            migrationBuilder.DropColumn(
                name: "Alert",
                table: "SmokeLogs");

            migrationBuilder.DropColumn(
                name: "AlertMessage",
                table: "SmokeLogs");
        }
    }
}
