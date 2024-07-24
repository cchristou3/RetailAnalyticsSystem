using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableMinerAssocRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MinerAssocRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeftHand = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    RightHand = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    Confidence = table.Column<double>(type: "float", nullable: false),
                    Support = table.Column<double>(type: "float", nullable: false),
                    Lift = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinerAssocRules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinerAssocRules");
        }
    }
}
