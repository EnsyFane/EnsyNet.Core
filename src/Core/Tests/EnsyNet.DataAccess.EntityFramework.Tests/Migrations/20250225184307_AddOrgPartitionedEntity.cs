using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsyNet.DataAccess.EntityFramework.Tests.Migrations;

/// <inheritdoc />
public partial class AddOrgPartitionedEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OrgPartitionedEntities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                OrgId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrgPartitionedEntities", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TestEntities_Id",
            table: "TestEntities",
            column: "Id",
            filter: "DeletedAt is NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OrgPartitionedEntities_Id",
            table: "OrgPartitionedEntities",
            column: "Id",
            filter: "DeletedAt is NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OrgPartitionedEntities_OrgId",
            table: "OrgPartitionedEntities",
            column: "OrgId",
            filter: "DeletedAt is NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OrgPartitionedEntities_OrgId_Name",
            table: "OrgPartitionedEntities",
            columns: new[] { "OrgId", "Name" },
            filter: "DeletedAt is NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrgPartitionedEntities");

        migrationBuilder.DropIndex(
            name: "IX_TestEntities_Id",
            table: "TestEntities");
    }
}
