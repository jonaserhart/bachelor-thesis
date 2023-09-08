using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlatModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalysisModels_AnalysisModelConfiguration_ConfigurationId",
                table: "AnalysisModels");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModelConfiguration_ConfigId",
                table: "GraphicalConfiguration");

            migrationBuilder.DropTable(
                name: "AnalysisModelConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisModels_ConfigurationId",
                table: "AnalysisModels");

            migrationBuilder.DropColumn(
                name: "Layout_H",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "Layout_W",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "Layout_X",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "Layout_Y",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "ConfigurationId",
                table: "AnalysisModels");

            migrationBuilder.RenameColumn(
                name: "ConfigId",
                table: "GraphicalConfiguration",
                newName: "ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalConfiguration_ConfigId",
                table: "GraphicalConfiguration",
                newName: "IX_GraphicalConfiguration_ModelId");

            migrationBuilder.CreateTable(
                name: "GraphicalReportItemLayout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    I = table.Column<Guid>(type: "uuid", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    W = table.Column<int>(type: "integer", nullable: false),
                    H = table.Column<int>(type: "integer", nullable: false),
                    ConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalReportItemLayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalReportItemLayout_GraphicalConfiguration_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "GraphicalConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemLayout_ConfigId",
                table: "GraphicalReportItemLayout",
                column: "ConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModels_ModelId",
                table: "GraphicalConfiguration",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModels_ModelId",
                table: "GraphicalConfiguration");

            migrationBuilder.DropTable(
                name: "GraphicalReportItemLayout");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "GraphicalConfiguration",
                newName: "ConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalConfiguration_ModelId",
                table: "GraphicalConfiguration",
                newName: "IX_GraphicalConfiguration_ConfigId");

            migrationBuilder.AddColumn<int>(
                name: "Layout_H",
                table: "GraphicalReportItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Layout_W",
                table: "GraphicalReportItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Layout_X",
                table: "GraphicalReportItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Layout_Y",
                table: "GraphicalReportItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisModels_ConfigurationId",
                table: "AnalysisModels",
                column: "ConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalysisModels_AnalysisModelConfiguration_ConfigurationId",
                table: "AnalysisModels",
                column: "ConfigurationId",
                principalTable: "AnalysisModelConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModelConfiguration_ConfigId",
                table: "GraphicalConfiguration",
                column: "ConfigId",
                principalTable: "AnalysisModelConfiguration",
                principalColumn: "Id");
        }
    }
}
