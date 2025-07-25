using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionProduit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuantiteFromProduit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "Produits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "Produits",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
