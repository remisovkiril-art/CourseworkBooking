using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelChainAndHotelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelChainId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stars",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HotelChain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelChain", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_HotelChainId",
                table: "Hotels",
                column: "HotelChainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_HotelChain_HotelChainId",
                table: "Hotels",
                column: "HotelChainId",
                principalTable: "HotelChain",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_HotelChain_HotelChainId",
                table: "Hotels");

            migrationBuilder.DropTable(
                name: "HotelChain");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_HotelChainId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HotelChainId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Stars",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Hotels");
        }
    }
}
