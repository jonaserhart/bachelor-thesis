using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalReportItemLayout",
                table: "GraphicalReportItemLayout");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GraphicalReportItemLayout");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalReportItemLayout",
                table: "GraphicalReportItemLayout",
                column: "I");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GraphicalReportItemLayout",
                table: "GraphicalReportItemLayout");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GraphicalReportItemLayout",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GraphicalReportItemLayout",
                table: "GraphicalReportItemLayout",
                column: "Id");
        }
    }
}
