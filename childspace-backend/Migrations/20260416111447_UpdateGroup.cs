using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace childspace_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Subjects_SubjectId",
                table: "Groups",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Subjects_SubjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SubjectId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Groups");
        }
    }
}
