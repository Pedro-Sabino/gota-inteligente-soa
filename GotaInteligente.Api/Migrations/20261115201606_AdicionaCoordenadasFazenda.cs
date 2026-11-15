using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GotaInteligente.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCoordenadasFazenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Fazendas",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Fazendas",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Fazendas");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Fazendas");
        }
    }
}
