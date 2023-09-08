using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedGraphicalItemContraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxH",
                table: "GraphicalReportItemLayout",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxW",
                table: "GraphicalReportItemLayout",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinH",
                table: "GraphicalReportItemLayout",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinW",
                table: "GraphicalReportItemLayout",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxH",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "MaxW",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "MinH",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "MinW",
                table: "GraphicalReportItemLayout");
        }
    }
}
