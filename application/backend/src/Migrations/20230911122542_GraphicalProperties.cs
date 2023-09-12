using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class GraphicalProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expressions_KPIs_LeftId",
                table: "Expressions");

            migrationBuilder.DropForeignKey(
                name: "FK_Expressions_KPIs_RightId",
                table: "Expressions");

            migrationBuilder.CreateTable(
                name: "GraphicalReportItemProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListFields = table.Column<string>(type: "jsonb", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalReportItemProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalReportItemProperties_GraphicalReportItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GraphicalReportItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemProperties_ItemId",
                table: "GraphicalReportItemProperties",
                column: "ItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expressions_KPIs_LeftId",
                table: "Expressions",
                column: "LeftId",
                principalTable: "KPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Expressions_KPIs_RightId",
                table: "Expressions",
                column: "RightId",
                principalTable: "KPIs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expressions_KPIs_LeftId",
                table: "Expressions");

            migrationBuilder.DropForeignKey(
                name: "FK_Expressions_KPIs_RightId",
                table: "Expressions");

            migrationBuilder.DropTable(
                name: "GraphicalReportItemProperties");

            migrationBuilder.AddForeignKey(
                name: "FK_Expressions_KPIs_LeftId",
                table: "Expressions",
                column: "LeftId",
                principalTable: "KPIs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expressions_KPIs_RightId",
                table: "Expressions",
                column: "RightId",
                principalTable: "KPIs",
                principalColumn: "Id");
        }
    }
}
