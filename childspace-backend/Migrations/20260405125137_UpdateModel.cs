using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace childspace_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Materials;");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "Materials",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "Materials",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Materials_SubjectId",
                table: "Materials",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Subjects_SubjectId",
                table: "Materials",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Subjects_SubjectId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_SubjectId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Materials");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "Materials",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Groups_GroupId",
                table: "Materials",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
