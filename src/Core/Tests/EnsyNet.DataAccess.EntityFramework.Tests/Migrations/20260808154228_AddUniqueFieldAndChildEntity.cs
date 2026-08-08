using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsyNet.DataAccess.EntityFramework.Tests.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueFieldAndChildEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueField",
                table: "TestEntities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChildTestEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildTestEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildTestEntities_TestEntities_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TestEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestEntities_UniqueField",
                table: "TestEntities",
                column: "UniqueField",
                unique: true,
                filter: "[UniqueField] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChildTestEntities_ParentId",
                table: "ChildTestEntities",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildTestEntities");

            migrationBuilder.DropIndex(
                name: "IX_TestEntities_UniqueField",
                table: "TestEntities");

            migrationBuilder.DropColumn(
                name: "UniqueField",
                table: "TestEntities");
        }
    }
}
