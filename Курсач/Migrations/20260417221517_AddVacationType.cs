using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Курсач.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VacationType",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VacationType",
                table: "Employees");
        }
    }
}
