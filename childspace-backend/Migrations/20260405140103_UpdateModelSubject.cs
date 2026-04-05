using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace childspace_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Subjects");
        }
    }
}
