using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Project.Migrations
{
    public partial class pet_model_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
