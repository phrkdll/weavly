using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavly.Configuration.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 26, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Module = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    StringValue = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    DoubleValue = table.Column<double>(type: "REAL", nullable: true),
                    IntValue = table.Column<int>(type: "INTEGER", nullable: true),
                    BoolValue = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    TouchedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TouchedBy = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Configuration");
        }
    }
}
