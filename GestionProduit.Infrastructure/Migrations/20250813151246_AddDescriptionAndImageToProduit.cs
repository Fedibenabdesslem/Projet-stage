using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionProduit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAndImageToProduit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Produits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Produits",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Produits");
        }
    }
}
