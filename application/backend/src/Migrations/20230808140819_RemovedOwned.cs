using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedOwned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_ConfigId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropIndex(
                name: "IX_GraphicalReportItemLayout_ConfigId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "ConfigId",
                table: "GraphicalReportItemLayout");

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
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_Graphical~",
                table: "GraphicalReportItemLayout",
                column: "GraphicalConfigurationId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_Graphical~",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropIndex(
                name: "IX_GraphicalReportItemLayout_GraphicalConfigurationId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "GraphicalConfigurationId",
                table: "GraphicalReportItemLayout");

            migrationBuilder.AddColumn<Guid>(
                name: "ConfigId",
                table: "GraphicalReportItemLayout",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemLayout_ConfigId",
                table: "GraphicalReportItemLayout",
                column: "ConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItemLayout_GraphicalConfigurations_ConfigId",
                table: "GraphicalReportItemLayout",
                column: "ConfigId",
                principalTable: "GraphicalConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
