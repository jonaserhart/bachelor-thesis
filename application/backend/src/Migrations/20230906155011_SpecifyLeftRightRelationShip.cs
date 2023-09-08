using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class SpecifyLeftRightRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Expressions_LeftId",
                table: "Expressions");

            migrationBuilder.DropIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions");

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
            migrationBuilder.DropIndex(
                name: "IX_Expressions_LeftId",
                table: "Expressions");

            migrationBuilder.DropIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions");

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_LeftId",
                table: "Expressions",
                column: "LeftId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions",
                column: "RightId",
                unique: true);
        }
    }
}
