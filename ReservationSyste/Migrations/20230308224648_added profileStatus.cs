using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSyste.Migrations
{
    public partial class addedprofileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileStatus",
                table: "PersonalProfileRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileStatus",
                table: "PersonalProfileRooms");
        }
    }
}
