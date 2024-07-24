using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPackTypeRecords");

            migrationBuilder.DropTable(
                name: "SalesByCityRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Segment",
                table: "Customers",
                type: "varchar(150)",
                unicode: false,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RfmScore",
                table: "Customers",
                type: "char(3)",
                unicode: false,
                fixedLength: true,
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(150)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerDistributionBySegmentRecords",
                columns: table => new
                {
                    SegmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfCustomers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SalesByProductPackTypeRecords",
                columns: table => new
                {
                    ProductPackTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SalesByProductTagsRecords",
                columns: table => new
                {
                    ProductTagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TopProfitableProductsPerTagsRecords",
                columns: table => new
                {
                    ProductTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Contribution = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TopSellingCitiesRecords",
                columns: table => new
                {
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "CustomerDistributionBySegmentRecords");

            migrationBuilder.DropTable(
                name: "SalesByProductPackTypeRecords");

            migrationBuilder.DropTable(
                name: "SalesByProductTagsRecords");

            migrationBuilder.DropTable(
                name: "TopProfitableProductsPerTagsRecords");

            migrationBuilder.DropTable(
                name: "TopSellingCitiesRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Segment",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RfmScore",
                table: "Customers",
                type: "char(150)",
                unicode: false,
                fixedLength: true,
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductPackTypeRecords",
                columns: table => new
                {
                    ProductPackTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
        }
    }
}
