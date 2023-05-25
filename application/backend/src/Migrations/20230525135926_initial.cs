using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
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
                name: "AnalysisModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisModels_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnalysisModels_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
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
                name: "Queries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferencedId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queries_AnalysisModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AnalysisModels",
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
                name: "Clause",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QueryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentClauseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Field = table.Column<string>(type: "text", nullable: false),
                    IsFieldValue = table.Column<bool>(type: "boolean", nullable: false),
                    FieldValue = table.Column<string>(type: "text", nullable: false),
                    LogicalOperator = table.Column<int>(type: "integer", nullable: false),
                    Operator_Name = table.Column<string>(type: "text", nullable: true),
                    Operator_ReferenceName = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clause", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clause_Clause_ParentClauseId",
                        column: x => x.ParentClauseId,
                        principalTable: "Clause",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clause_Queries_QueryId",
                        column: x => x.QueryId,
                        principalTable: "Queries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FieldInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    QueryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldInfos_Queries_QueryId",
                        column: x => x.QueryId,
                        principalTable: "Queries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisModels_ProjectId",
                table: "AnalysisModels",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisModels_TeamId",
                table: "AnalysisModels",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Clause_ParentClauseId",
                table: "Clause",
                column: "ParentClauseId");

            migrationBuilder.CreateIndex(
                name: "IX_Clause_QueryId",
                table: "Clause",
                column: "QueryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldInfos_QueryId",
                table: "FieldInfos",
                column: "QueryId");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_ModelId",
                table: "Queries",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModels_ModelId",
                table: "UserModels",
                column: "ModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clause");

            migrationBuilder.DropTable(
                name: "FieldInfos");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UserModels");

            migrationBuilder.DropTable(
                name: "Queries");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AnalysisModels");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Team");
        }
    }
}
