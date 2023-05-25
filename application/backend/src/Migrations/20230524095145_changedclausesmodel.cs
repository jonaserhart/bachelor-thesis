using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class changedclausesmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Condition");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferencedId",
                table: "Queries",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Clause_ParentClauseId",
                table: "Clause",
                column: "ParentClauseId");

            migrationBuilder.CreateIndex(
                name: "IX_Clause_QueryId",
                table: "Clause",
                column: "QueryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clause");

            migrationBuilder.DropColumn(
                name: "ReferencedId",
                table: "Queries");

            migrationBuilder.CreateTable(
                name: "Condition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QueryId = table.Column<Guid>(type: "uuid", nullable: true),
                    AndOr = table.Column<string>(type: "text", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: true)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Condition_Queries_QueryId",
                        column: x => x.QueryId,
                        principalTable: "Queries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Condition_QueryId",
                table: "Condition",
                column: "QueryId");
        }
    }
}
