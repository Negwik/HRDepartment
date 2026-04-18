using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Курсач.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildrenCount",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChildrenInfo",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Disability",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DisabilityGroup",
                table: "Employees",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Education",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EducationalInstitution",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GraduationYear",
                table: "Employees",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Employees",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenCount",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ChildrenInfo",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Disability",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DisabilityGroup",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationalInstitution",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Employees");
        }
    }
}
