using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsyNet.DataAccess.EntityFramework.Tests.Migrations
{
    /// <inheritdoc />
    public partial class FixPartitionedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "PartitionedTestEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "PartitionedTestEntities");
        }
    }
}
