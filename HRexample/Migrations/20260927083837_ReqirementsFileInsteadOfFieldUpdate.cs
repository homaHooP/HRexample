using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRexample.Migrations
{
    /// <inheritdoc />
    public partial class ReqirementsFileInsteadOfFieldUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "JobRequirements");

            migrationBuilder.RenameColumn(
                name: "RequiredSkills",
                table: "JobRequirements",
                newName: "Position");

            migrationBuilder.AddColumn<string>(
                name: "s3FileKey",
                table: "JobRequirements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "s3FileKey",
                table: "JobRequirements");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "JobRequirements",
                newName: "RequiredSkills");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "JobRequirements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
