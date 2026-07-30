using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoFleetLogistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorShipmentWithMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_CompanyId",
                table: "Shipments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_CreatedById",
                table: "Shipments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DriverId",
                table: "Shipments",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Companies_CompanyId",
                table: "Shipments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Users_CreatedById",
                table: "Shipments",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Users_DriverId",
                table: "Shipments",
                column: "DriverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Companies_CompanyId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Users_CreatedById",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Users_DriverId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_CompanyId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_CreatedById",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_DriverId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Shipments");
        }
    }
}
