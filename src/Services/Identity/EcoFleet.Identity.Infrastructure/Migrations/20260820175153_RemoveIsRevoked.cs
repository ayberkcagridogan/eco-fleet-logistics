using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoFleet.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsRevoked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedAt",
                schema: "Identity",
                table: "RefreshTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                schema: "Identity",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);
        }
    }
}
