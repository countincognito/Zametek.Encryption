using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zametek.Access.Encryption.Tests.Migrations
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
                    SymmetricKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsymmetricKeyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SymmetricKeyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AsymmetricKeyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AsymmetricKeyVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WrappedSymmetricKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitializationVector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
