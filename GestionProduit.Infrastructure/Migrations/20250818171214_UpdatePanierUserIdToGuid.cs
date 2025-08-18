using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionProduit.Infrastructure.Migrations
{
    public partial class UpdatePanierUserIdToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Supprimer les anciennes contraintes/fk temporaires
            migrationBuilder.DropForeignKey(
                name: "FK_PanierItems_Users_UserId1",
                table: "PanierItems");

            migrationBuilder.DropIndex(
                name: "IX_PanierItems_UserId1",
                table: "PanierItems");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PanierItems");

            // Conversion de la colonne UserId en uuid avec génération de nouveaux GUID
            migrationBuilder.Sql(
                @"ALTER TABLE ""PanierItems"" 
                  ALTER COLUMN ""UserId"" TYPE uuid USING gen_random_uuid();");

            // Créer un index et une FK sur la nouvelle colonne Guid
            migrationBuilder.CreateIndex(
                name: "IX_PanierItems_UserId",
                table: "PanierItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PanierItems_Users_UserId",
                table: "PanierItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PanierItems_Users_UserId",
                table: "PanierItems");

            migrationBuilder.DropIndex(
                name: "IX_PanierItems_UserId",
                table: "PanierItems");

            // Revenir à int (attention, les GUID existants seront perdus)
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PanierItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "PanierItems",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_PanierItems_UserId1",
                table: "PanierItems",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PanierItems_Users_UserId1",
                table: "PanierItems",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
