using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSyste.Migrations
{
    public partial class Addedpurposefeild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "PersonalProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "PersonalProfiles");
        }
    }
}
