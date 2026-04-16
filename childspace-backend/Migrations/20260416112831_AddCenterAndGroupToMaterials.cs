using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace childspace_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCenterAndGroupToMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials");

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "Materials",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CenterId",
                table: "Materials",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Centers_CenterId",
                table: "Materials",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Centers_CenterId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_CenterId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Materials");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
