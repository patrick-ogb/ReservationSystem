using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSyste.Migrations
{
    public partial class addedRoomIdtoTransctionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Transactions");
        }
    }
}
