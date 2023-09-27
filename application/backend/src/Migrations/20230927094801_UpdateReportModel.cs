using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ReportData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    QueryResults = table.Column<string>(type: "jsonb", nullable: false),
                    KPIsAndValues = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportData_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportData_ReportId",
                table: "ReportData",
                column: "ReportId",
                unique: true);

            // Migrate legacy report data
            migrationBuilder.Sql(@"
                CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
                INSERT INTO ""ReportData"" (""Id"", ""ReportId"", ""QueryResults"", ""KPIsAndValues"")
                SELECT uuid_generate_v4(), ""Id"", ""QueryResults"", ""KPIsAndValues"" FROM ""Reports"";
            ");

            migrationBuilder.DropColumn(
                name: "KPIsAndValues",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "QueryResults",
                table: "Reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KPIsAndValues",
                table: "Reports",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QueryResults",
                table: "Reports",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            // Migrate to legacy report data
            migrationBuilder.Sql(@"
                UPDATE ""Reports"" r
                SET ""QueryResults"" = rd. ""QueryResults"" ,
                    ""KPIsAndValues"" = rd.""KPIsAndValues""
                FROM ""ReportData"" rd
                WHERE r.""Id"" = rd.""ReportId""
            ");

            migrationBuilder.DropTable(
                name: "ReportData");
        }
    }
}
