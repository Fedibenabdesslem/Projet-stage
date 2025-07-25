using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionProduit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantiteToProduit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "Produits",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "Produits");
        }
    }
}
