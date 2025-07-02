using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavly.Auth.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(
                        type: "character varying(1024)",
                        maxLength: 1024,
                        nullable: false
                    ),
                    UserName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoginRequested = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    TouchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TouchedBy = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Value = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AppUserId = table.Column<string>(type: "character varying(26)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_User_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "User",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    RolesId = table.Column<string>(type: "character varying(26)", nullable: false),
                    UsersId = table.Column<string>(type: "character varying(26)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserRole_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(name: "IX_Token_AppUserId", table: "Token", column: "AppUserId");

            migrationBuilder.CreateIndex(name: "IX_UserRole_UsersId", table: "UserRole", column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Token");

            migrationBuilder.DropTable(name: "UserRole");

            migrationBuilder.DropTable(name: "Role");

            migrationBuilder.DropTable(name: "User");
        }
    }
}
