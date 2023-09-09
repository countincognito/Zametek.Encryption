using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zametek.Access.Encryption.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SymmetricKeys",
                columns: table => new
                {
                    SymmetricKeyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AsymmetricKeyId = table.Column<string>(type: "text", nullable: false),
                    SymmetricKeyName = table.Column<string>(type: "text", nullable: false),
                    AsymmetricKeyName = table.Column<string>(type: "text", nullable: false),
                    AsymmetricKeyVersion = table.Column<string>(type: "text", nullable: false),
                    WrappedSymmetricKey = table.Column<string>(type: "text", nullable: false),
                    InitializationVector = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymmetricKeys", x => new { x.SymmetricKeyId, x.AsymmetricKeyId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SymmetricKeys");
        }
    }
}
