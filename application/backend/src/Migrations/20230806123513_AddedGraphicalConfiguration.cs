using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedGraphicalConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConfigurationId",
                table: "AnalysisModels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AnalysisModelConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisModelConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraphicalConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ConfigId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalConfiguration_AnalysisModelConfiguration_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "AnalysisModelConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GraphicalReportItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Layout_I = table.Column<string>(type: "text", nullable: false),
                    Layout_X = table.Column<int>(type: "integer", nullable: false),
                    Layout_Y = table.Column<int>(type: "integer", nullable: false),
                    Layout_W = table.Column<int>(type: "integer", nullable: false),
                    Layout_H = table.Column<int>(type: "integer", nullable: false),
                    GraphicalConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSources = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalReportItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                        column: x => x.GraphicalConfigId,
                        principalTable: "GraphicalConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisModels_ConfigurationId",
                table: "AnalysisModels",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalConfiguration_ConfigId",
                table: "GraphicalConfiguration",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItem_GraphicalConfigId",
                table: "GraphicalReportItem",
                column: "GraphicalConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisModels_AnalysisModelConfiguration_ConfigurationId",
                table: "AnalysisModels",
                column: "ConfigurationId",
                principalTable: "AnalysisModelConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisModels_AnalysisModelConfiguration_ConfigurationId",
                table: "AnalysisModels");

            migrationBuilder.DropTable(
                name: "GraphicalReportItem");

            migrationBuilder.DropTable(
                name: "GraphicalConfiguration");

            migrationBuilder.DropTable(
                name: "AnalysisModelConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisModels_ConfigurationId",
                table: "AnalysisModels");

            migrationBuilder.DropColumn(
                name: "ConfigurationId",
                table: "AnalysisModels");
        }
    }
}
