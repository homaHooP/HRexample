using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRexample.Migrations
{
    /// <inheritdoc />
    public partial class Bugg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelResponseId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications",
                column: "ModelResponseId",
                principalTable: "ModelResponses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelResponseId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications",
                column: "ModelResponseId",
                principalTable: "ModelResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
