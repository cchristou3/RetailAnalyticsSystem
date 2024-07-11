using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "PromotionTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,3)", precision: 15, scale: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionTypes_InvoiceId",
                table: "PromotionTypes",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Name",
                table: "Invoice",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionTypes_Invoice_InvoiceId",
                table: "PromotionTypes",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionTypes_Invoice_InvoiceId",
                table: "PromotionTypes");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_PromotionTypes_InvoiceId",
                table: "PromotionTypes");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "PromotionTypes");
        }
    }
}
