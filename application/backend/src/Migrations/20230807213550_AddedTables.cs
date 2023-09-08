using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModels_ModelId",
                table: "GraphicalConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfiguration_ConfigId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalReportItem",
                table: "GraphicalReportItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalConfiguration",
                table: "GraphicalConfiguration");

            migrationBuilder.RenameTable(
                name: "GraphicalReportItem",
                newName: "GraphicalReportItems");

            migrationBuilder.RenameTable(
                name: "GraphicalConfiguration",
                newName: "GraphicalConfigurations");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalReportItem_GraphicalConfigId",
                table: "GraphicalReportItems",
                newName: "IX_GraphicalReportItems_GraphicalConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalConfiguration_ModelId",
                table: "GraphicalConfigurations",
                newName: "IX_GraphicalConfigurations_ModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalReportItems",
                table: "GraphicalReportItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalConfigurations",
                table: "GraphicalConfigurations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_ConfigId",
                table: "GraphicalReportItemLayout",
                column: "ConfigId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                table: "GraphicalConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_ConfigId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                table: "GraphicalReportItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalReportItems",
                table: "GraphicalReportItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalConfigurations",
                table: "GraphicalConfigurations");

            migrationBuilder.RenameTable(
                name: "GraphicalReportItems",
                newName: "GraphicalReportItem");

            migrationBuilder.RenameTable(
                name: "GraphicalConfigurations",
                newName: "GraphicalConfiguration");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalReportItems_GraphicalConfigId",
                table: "GraphicalReportItem",
                newName: "IX_GraphicalReportItem_GraphicalConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_GraphicalConfigurations_ModelId",
                table: "GraphicalConfiguration",
                newName: "IX_GraphicalConfiguration_ModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalReportItem",
                table: "GraphicalReportItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalConfiguration",
                table: "GraphicalConfiguration",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalConfiguration_AnalysisModels_ModelId",
                table: "GraphicalConfiguration",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfiguration",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfiguration_ConfigId",
                table: "GraphicalReportItemLayout",
                column: "ConfigId",
                principalTable: "GraphicalConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
