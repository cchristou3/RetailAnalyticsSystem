using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesRemoveColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AREA_ID",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "AREA_NAME",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "BRAND",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DIET",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "OUTLET_ID",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "OUTLET_NAME",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "OUTLET_TYPE_ID",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "OUTLET_TYPE_NAME",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PACK_TYPE",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PAREA_ID",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PROMOTION",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "URBAN_RURAL",
                table: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AREA_ID",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AREA_NAME",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BRAND",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DIET",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OUTLET_ID",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OUTLET_NAME",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OUTLET_TYPE_ID",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OUTLET_TYPE_NAME",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PACK_TYPE",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PAREA_ID",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PROMOTION",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URBAN_RURAL",
                table: "Sales",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
