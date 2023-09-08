using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class added_DoIfMultipleExpressions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Connection",
                table: "Expressions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpressionConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpressionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpressionId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Operator = table.Column<int>(type: "integer", nullable: false),
                    CompareValue = table.Column<string>(type: "text", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpressionConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpressionConditions_Expressions_ExpressionId",
                        column: x => x.ExpressionId,
                        principalTable: "Expressions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpressionConditions_Expressions_ExpressionId1",
                        column: x => x.ExpressionId1,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpressionConditions_ExpressionId",
                table: "ExpressionConditions",
                column: "ExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpressionConditions_ExpressionId1",
                table: "ExpressionConditions",
                column: "ExpressionId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpressionConditions");

            migrationBuilder.DropColumn(
                name: "Connection",
                table: "Expressions");
        }
    }
}
