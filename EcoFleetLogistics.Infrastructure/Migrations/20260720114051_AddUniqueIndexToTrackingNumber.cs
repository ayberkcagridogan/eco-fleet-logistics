using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoFleetLogistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToTrackingNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments",
                column: "TrackingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments");
        }
    }
}
