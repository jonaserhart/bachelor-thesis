using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "UserModels");

            migrationBuilder.AddColumn<int>(
                name: "Permission",
                table: "UserModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permission",
                table: "UserModels");

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "UserModels",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
