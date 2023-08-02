using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    EMail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "EXTRACT(EPOCH FROM NOW())::BIGINT"),
                    QueryResults = table.Column<string>(type: "jsonb", nullable: false),
                    KPIsAndValues = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AnalysisModels_AnalysisModelId",
                        column: x => x.AnalysisModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserModels",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Permissions = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModels", x => new { x.UserId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_UserModels_AnalysisModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expressions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    QueryId = table.Column<string>(type: "text", nullable: true),
                    AvgExpression_Field = table.Column<string>(type: "text", nullable: true),
                    Operator = table.Column<int>(type: "integer", nullable: true),
                    CompareValue = table.Column<string>(type: "text", nullable: true),
                    Field = table.Column<string>(type: "text", nullable: true),
                    LeftId = table.Column<Guid>(type: "uuid", nullable: true),
                    RightId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaxExpression_Field = table.Column<string>(type: "text", nullable: true),
                    MinExpression_Field = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    SumExpression_Field = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expressions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    ExpressionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ShowInReport = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptableValues = table.Column<string>(type: "text", nullable: false),
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
                name: "IX_Expressions_LeftId",
                table: "Expressions",
                column: "LeftId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions",
                column: "RightId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_AnalysisModelId",
                table: "KPIs",
                column: "AnalysisModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_ExpressionId",
                table: "KPIs",
                column: "ExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AnalysisModelId",
                table: "Reports",
                column: "AnalysisModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModels_ModelId",
                table: "UserModels",
                column: "ModelId");

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
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "UserModels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "KPIs");

            migrationBuilder.DropTable(
                name: "AnalysisModels");

            migrationBuilder.DropTable(
                name: "Expressions");
        }
    }
}
