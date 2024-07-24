using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSalesByProductPackageType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contribution",
                table: "ProductPackTypeRecords");

            migrationBuilder.RenameColumn(
                name: "Sales",
                table: "TopSellingProducts",
                newName: "Volume");

            migrationBuilder.RenameColumn(
                name: "Sales",
                table: "ProductPackTypeRecords",
                newName: "Value");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "TopSellingProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Volume",
                table: "ProductPackTypeRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "TopSellingProducts");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "ProductPackTypeRecords");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "TopSellingProducts",
                newName: "Sales");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProductPackTypeRecords",
                newName: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "Contribution",
                table: "ProductPackTypeRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
