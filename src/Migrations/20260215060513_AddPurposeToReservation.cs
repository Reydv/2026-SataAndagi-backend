using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2026_SataAndagi_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPurposeToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Reservations");
        }
    }
}
