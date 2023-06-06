using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addedkpis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ExpressionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnalysisModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                        column: x => x.AnalysisModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KPIs_Expressions_ExpressionId",
                        column: x => x.ExpressionId,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_AnalysisModelId",
                table: "KPIs",
                column: "AnalysisModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_ExpressionId",
                table: "KPIs",
                column: "ExpressionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KPIs");
        }
    }
}
