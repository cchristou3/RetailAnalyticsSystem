using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireWebApp.SpaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSegmentMinerAssocRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Segment",
                table: "MinerAssocRules",
                type: "varchar(150)",
                unicode: false,
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Segment",
                table: "MinerAssocRules");
        }
    }
}
