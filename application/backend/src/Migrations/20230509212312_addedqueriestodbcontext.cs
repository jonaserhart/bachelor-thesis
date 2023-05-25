using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addedqueriestodbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Condition_Query_QueryId",
                table: "Condition");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldInfo_Query_QueryId",
                table: "FieldInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Query_AnalysisModels_ModelId",
                table: "Query");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Query",
                table: "Query");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldInfo",
                table: "FieldInfo");

            migrationBuilder.RenameTable(
                name: "Query",
                newName: "Queries");

            migrationBuilder.RenameTable(
                name: "FieldInfo",
                newName: "FieldInfos");

            migrationBuilder.RenameIndex(
                name: "IX_Query_ModelId",
                table: "Queries",
                newName: "IX_Queries_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldInfo_QueryId",
                table: "FieldInfos",
                newName: "IX_FieldInfos_QueryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Queries",
                table: "Queries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldInfos",
                table: "FieldInfos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Condition_Queries_QueryId",
                table: "Condition",
                column: "QueryId",
                principalTable: "Queries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldInfos_Queries_QueryId",
                table: "FieldInfos",
                column: "QueryId",
                principalTable: "Queries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_AnalysisModels_ModelId",
                table: "Queries",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Condition_Queries_QueryId",
                table: "Condition");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldInfos_Queries_QueryId",
                table: "FieldInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_Queries_AnalysisModels_ModelId",
                table: "Queries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Queries",
                table: "Queries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FieldInfos",
                table: "FieldInfos");

            migrationBuilder.RenameTable(
                name: "Queries",
                newName: "Query");

            migrationBuilder.RenameTable(
                name: "FieldInfos",
                newName: "FieldInfo");

            migrationBuilder.RenameIndex(
                name: "IX_Queries_ModelId",
                table: "Query",
                newName: "IX_Query_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_FieldInfos_QueryId",
                table: "FieldInfo",
                newName: "IX_FieldInfo_QueryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Query",
                table: "Query",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FieldInfo",
                table: "FieldInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Condition_Query_QueryId",
                table: "Condition",
                column: "QueryId",
                principalTable: "Query",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldInfo_Query_QueryId",
                table: "FieldInfo",
                column: "QueryId",
                principalTable: "Query",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Query_AnalysisModels_ModelId",
                table: "Query",
                column: "ModelId",
                principalTable: "AnalysisModels",
                principalColumn: "Id");
        }
    }
}
