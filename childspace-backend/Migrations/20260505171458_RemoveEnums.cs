using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace childspace_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SubscriptionStatus",
                table: "Centers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionStatus",
                table: "Centers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
