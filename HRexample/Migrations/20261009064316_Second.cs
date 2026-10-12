using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRexample.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobRequirementId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ModelResponseId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ModelResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Experience = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PercentageFitForJob = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelResponses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobRequirementId",
                table: "Applications",
                column: "JobRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ModelResponseId",
                table: "Applications",
                column: "ModelResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_JobRequirements_JobRequirementId",
                table: "Applications",
                column: "JobRequirementId",
                principalTable: "JobRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications",
                column: "ModelResponseId",
                principalTable: "ModelResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_JobRequirements_JobRequirementId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_ModelResponses_ModelResponseId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "ModelResponses");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobRequirementId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ModelResponseId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "JobRequirementId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ModelResponseId",
                table: "Applications");
        }
    }
}
