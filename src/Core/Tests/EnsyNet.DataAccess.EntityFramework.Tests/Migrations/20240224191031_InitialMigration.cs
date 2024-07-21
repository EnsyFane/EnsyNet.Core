using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsyNet.DataAccess.EntityFramework.Tests.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TestEntities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                StringField = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IntField = table.Column<int>(type: "int", nullable: false),
                BoolField = table.Column<bool>(type: "bit", nullable: false),
                FloatField = table.Column<float>(type: "real", nullable: false),
                DoubleField = table.Column<double>(type: "float", nullable: false),
                DecimalField = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DateTimeField = table.Column<DateTime>(type: "datetime2", nullable: false),
                TimeSpanField = table.Column<TimeSpan>(type: "time", nullable: false),
                GuidField = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TestEntities", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TestEntities");
    }
}
