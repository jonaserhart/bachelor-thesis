using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addedexpressions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expressions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    FieldExpressionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Field = table.Column<string>(type: "text", nullable: true),
                    Operator = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    LeftId = table.Column<Guid>(type: "uuid", nullable: true),
                    RightId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldExpression_Field = table.Column<string>(type: "text", nullable: true),
                    NumericValueExpression_Value = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expressions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expressions_Expressions_FieldExpressionId",
                        column: x => x.FieldExpressionId,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expressions_Expressions_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expressions_Expressions_RightId",
                        column: x => x.RightId,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_FieldExpressionId",
                table: "Expressions",
                column: "FieldExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_LeftId",
                table: "Expressions",
                column: "LeftId");

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions",
                column: "RightId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expressions");
        }
    }
}
