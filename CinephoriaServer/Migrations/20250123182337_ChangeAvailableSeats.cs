using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinephoriaServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAvailableSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Showtimes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Showtimes");
        }
    }
}
