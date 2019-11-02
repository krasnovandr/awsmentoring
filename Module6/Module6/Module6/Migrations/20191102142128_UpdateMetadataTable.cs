using Microsoft.EntityFrameworkCore.Migrations;

namespace Module6.Migrations
{
    public partial class UpdateMetadataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileSile",
                table: "Metadata",
                newName: "FileSize");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Metadata",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Metadata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Metadata");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Metadata");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                table: "Metadata",
                newName: "FileSile");
        }
    }
}
