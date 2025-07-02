using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavly.Configuration.Persistence.Migrations.Postgres
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
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Module = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StringValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DoubleValue = table.Column<double>(type: "double precision", nullable: true),
                    IntValue = table.Column<int>(type: "integer", nullable: true),
                    BoolValue = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TouchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TouchedBy = table.Column<string>(type: "text", nullable: true),
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
