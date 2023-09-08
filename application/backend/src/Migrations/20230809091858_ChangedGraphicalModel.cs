using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedGraphicalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_Graphical~",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems");

            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                table: "KPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AnalysisModels_AnalysisModelId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_GraphicalReportItemLayout_GraphicalConfigurationId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "DataSources",
                table: "GraphicalReportItems");

            migrationBuilder.DropColumn(
                name: "GraphicalConfigurationId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.CreateTable(
                name: "GraphicalItemDataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    KPIs = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalItemDataSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalItemDataSources_GraphicalReportItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GraphicalReportItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemLayout_I",
                table: "GraphicalReportItemLayout",
                column: "I",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalItemDataSources_ItemId",
                table: "GraphicalItemDataSources",
                column: "ItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalReportItems_I",
                table: "GraphicalReportItemLayout",
                column: "I",
                principalTable: "GraphicalReportItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                table: "KPIs",
                column: "AnalysisModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AnalysisModels_AnalysisModelId",
                table: "Reports",
                column: "AnalysisModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalReportItems_I",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems");

            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                table: "KPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AnalysisModels_AnalysisModelId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "GraphicalItemDataSources");

            migrationBuilder.DropIndex(
                name: "IX_GraphicalReportItemLayout_I",
                table: "GraphicalReportItemLayout");

            migrationBuilder.AddColumn<string>(
                name: "DataSources",
                table: "GraphicalReportItems",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GraphicalConfigurationId",
                table: "GraphicalReportItemLayout",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemLayout_GraphicalConfigurationId",
                table: "GraphicalReportItemLayout",
                column: "GraphicalConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_Graphical~",
                table: "GraphicalReportItemLayout",
                column: "GraphicalConfigurationId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                table: "KPIs",
                column: "AnalysisModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AnalysisModels_AnalysisModelId",
                table: "Reports",
                column: "AnalysisModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");
        }
    }
}
