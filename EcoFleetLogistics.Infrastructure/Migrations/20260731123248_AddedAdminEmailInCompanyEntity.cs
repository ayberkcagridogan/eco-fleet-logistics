using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoFleetLogistics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAdminEmailInCompanyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminEmail",
                table: "Companies");
        }
    }
}
