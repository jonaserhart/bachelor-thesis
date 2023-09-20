using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                name: "GraphicalConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalConfigurations_AnalysisModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalysisModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelAssociationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedById = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedAt = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "EXTRACT(EPOCH FROM NOW())::BIGINT"),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<long>(type: "bigint", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelAssociationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelAssociationRequests_AnalysisModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelAssociationRequests_Users_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Permission = table.Column<int>(type: "integer", nullable: false)
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
                name: "GraphicalReportItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GraphicalConfigId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalReportItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalReportItems_GraphicalConfigurations_GraphicalConfi~",
                        column: x => x.GraphicalConfigId,
                        principalTable: "GraphicalConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraphicalItemDataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    KPIs = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalItemDataSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalItemDataSources_GraphicalReportItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GraphicalReportItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraphicalReportItemLayout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    I = table.Column<Guid>(type: "uuid", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    W = table.Column<int>(type: "integer", nullable: false),
                    H = table.Column<int>(type: "integer", nullable: false),
                    MaxW = table.Column<int>(type: "integer", nullable: true),
                    MaxH = table.Column<int>(type: "integer", nullable: true),
                    MinW = table.Column<int>(type: "integer", nullable: true),
                    MinH = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraphicalReportItemLayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraphicalReportItemLayout_GraphicalReportItems_I",
                        column: x => x.I,
                        principalTable: "GraphicalReportItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    Connection = table.Column<int>(type: "integer", nullable: true),
                    ExtractField = table.Column<string>(type: "text", nullable: true),
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
                    AcceptableValues = table.Column<string>(type: "text", nullable: true, defaultValue: "any"),
                    AnalysisModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIs_AnalysisModels_AnalysisModelId",
                        column: x => x.AnalysisModelId,
                        principalTable: "AnalysisModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KPIs_Expressions_ExpressionId",
                        column: x => x.ExpressionId,
                        principalTable: "Expressions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KPIs_KPIFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "KPIFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpressionConditions_ExpressionId",
                table: "ExpressionConditions",
                column: "ExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpressionConditions_ExpressionId1",
                table: "ExpressionConditions",
                column: "ExpressionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_LeftId",
                table: "Expressions",
                column: "LeftId");

            migrationBuilder.CreateIndex(
                name: "IX_Expressions_RightId",
                table: "Expressions",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalConfigurations_ModelId",
                table: "GraphicalConfigurations",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalItemDataSources_ItemId",
                table: "GraphicalItemDataSources",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemLayout_I",
                table: "GraphicalReportItemLayout",
                column: "I",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItemProperties_ItemId",
                table: "GraphicalReportItemProperties",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GraphicalReportItems_GraphicalConfigId",
                table: "GraphicalReportItems",
                column: "GraphicalConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIFolders_ModelId",
                table: "KPIFolders",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIFolders_ParentFolderId",
                table: "KPIFolders",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_AnalysisModelId",
                table: "KPIs",
                column: "AnalysisModelId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_ExpressionId",
                table: "KPIs",
                column: "ExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_FolderId",
                table: "KPIs",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelAssociationRequests_IssuedById",
                table: "ModelAssociationRequests",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModelAssociationRequests_ModelId",
                table: "ModelAssociationRequests",
                column: "ModelId");

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
                name: "FK_ExpressionConditions_Expressions_ExpressionId",
                table: "ExpressionConditions",
                column: "ExpressionId",
                principalTable: "Expressions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpressionConditions_Expressions_ExpressionId1",
                table: "ExpressionConditions",
                column: "ExpressionId1",
                principalTable: "Expressions",
                principalColumn: "Id");

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
                name: "FK_KPIs_Expressions_ExpressionId",
                table: "KPIs");

            migrationBuilder.DropTable(
                name: "ExpressionConditions");

            migrationBuilder.DropTable(
                name: "GraphicalItemDataSources");

            migrationBuilder.DropTable(
                name: "GraphicalReportItemLayout");

            migrationBuilder.DropTable(
                name: "GraphicalReportItemProperties");

            migrationBuilder.DropTable(
                name: "ModelAssociationRequests");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "UserModels");

            migrationBuilder.DropTable(
                name: "GraphicalReportItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GraphicalConfigurations");

            migrationBuilder.DropTable(
                name: "Expressions");

            migrationBuilder.DropTable(
                name: "KPIs");

            migrationBuilder.DropTable(
                name: "KPIFolders");

            migrationBuilder.DropTable(
                name: "AnalysisModels");
        }
    }
}
