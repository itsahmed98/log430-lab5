using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventaireMcService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MagasinId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    QuantiteDemandee = table.Column<int>(type: "integer", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandes", x => x.DemandeId);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    MagasinId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => new { x.MagasinId, x.ProduitId });
                });

            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "MagasinId", "ProduitId", "Quantite" },
                values: new object[,]
                {
                    { 1, 1, 400 },
                    { 1, 2, 400 },
                    { 1, 3, 400 },
                    { 2, 1, 100 },
                    { 2, 2, 80 },
                    { 3, 1, 60 },
                    { 3, 3, 90 },
                    { 4, 2, 200 },
                    { 5, 1, 310 },
                    { 5, 2, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Demandes");

            migrationBuilder.DropTable(
                name: "StockItems");
        }
    }
}
