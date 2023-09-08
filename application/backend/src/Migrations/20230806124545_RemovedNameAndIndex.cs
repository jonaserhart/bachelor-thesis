using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNameAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "Layout_I",
                table: "GraphicalReportItem");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GraphicalReportItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "GraphicalConfigId",
                table: "GraphicalReportItem",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfiguration",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "GraphicalConfigId",
                table: "GraphicalReportItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Layout_I",
                table: "GraphicalReportItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GraphicalReportItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_GraphicalReportItem_GraphicalConfiguration_GraphicalConfigId",
                table: "GraphicalReportItem",
                column: "GraphicalConfigId",
                principalTable: "GraphicalConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
