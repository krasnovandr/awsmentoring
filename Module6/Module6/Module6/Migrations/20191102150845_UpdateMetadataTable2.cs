using Microsoft.EntityFrameworkCore.Migrations;

namespace Module6.Migrations
{
    public partial class UpdateMetadataTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Metadata");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Metadata",
                nullable: true);
        }
    }
}
