using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedKpiFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FolderId",
                table: "KPIs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KPIFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIFolders_AnalysisModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KPIFolders_KPIFolders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "KPIFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_FolderId",
                table: "KPIs",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIFolders_ModelId",
                table: "KPIFolders",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIFolders_ParentFolderId",
                table: "KPIFolders",
                column: "ParentFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_KPIFolders_FolderId",
                table: "KPIs",
                column: "FolderId",
                principalTable: "KPIFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_KPIFolders_FolderId",
                table: "KPIs");

            migrationBuilder.DropTable(
                name: "KPIFolders");

            migrationBuilder.DropIndex(
                name: "IX_KPIs_FolderId",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "KPIs");
        }
    }
}
