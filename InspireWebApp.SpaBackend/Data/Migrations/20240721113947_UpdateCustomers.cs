using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Contribution",
                table: "TopProfitableProductsPerPackTypeRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RfmScore",
                table: "Customers",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Segment",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerDistributionByCategoryRecords",
                columns: table => new
                {
                    CustomerCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfCustomers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "CustomerDistributionByCityRecords",
                columns: table => new
                {
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfCustomers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DaySalesRecords",
                columns: table => new
                {
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DayNo = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SalesByCityRecords",
                columns: table => new
                {
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SalesByCustomerCategoryRecords",
                columns: table => new
                {
                    CustomerCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerDistributionByCategoryRecords");

            migrationBuilder.DropTable(
                name: "CustomerDistributionByCityRecords");

            migrationBuilder.DropTable(
                name: "DaySalesRecords");

            migrationBuilder.DropTable(
                name: "SalesByCityRecords");

            migrationBuilder.DropTable(
                name: "SalesByCustomerCategoryRecords");

            migrationBuilder.DropColumn(
                name: "Contribution",
                table: "TopProfitableProductsPerPackTypeRecords");

            migrationBuilder.DropColumn(
                name: "RfmScore",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Segment",
                table: "Customers");
        }
    }
}
