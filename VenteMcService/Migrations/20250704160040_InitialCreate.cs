using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ventes",
                columns: table => new
                {
                    VenteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TypeVente = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    MagasinId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventes", x => x.VenteId);
                });

            migrationBuilder.CreateTable(
                name: "LignesVente",
                columns: table => new
                {
                    LigneVenteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenteId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<int>(type: "integer", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesVente", x => x.LigneVenteId);
                    table.ForeignKey(
                        name: "FK_LignesVente_Ventes_VenteId",
                        column: x => x.VenteId,
                        principalTable: "Ventes",
                        principalColumn: "VenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Ventes",
                columns: new[] { "VenteId", "Date", "MagasinId", "TypeVente" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 3, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1806), 1, "POS" },
                    { 2, new DateTime(2025, 6, 30, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1814), 2, "POS" },
                    { 3, new DateTime(2025, 7, 1, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1815), 1, "POS" },
                    { 4, new DateTime(2025, 7, 2, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1815), 3, "POS" },
                    { 5, new DateTime(2025, 6, 28, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1816), 2, "POS" },
                    { 6, new DateTime(2025, 6, 30, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1817), 1, "POS" },
                    { 7, new DateTime(2025, 6, 29, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1817), 3, "POS" }
                });

            migrationBuilder.InsertData(
                table: "Ventes",
                columns: new[] { "VenteId", "ClientId", "Date", "TypeVente" },
                values: new object[,]
                {
                    { 8, 1, new DateTime(2025, 7, 3, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1898), "ECommerce" },
                    { 9, 2, new DateTime(2025, 7, 2, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1900), "ECommerce" },
                    { 10, 3, new DateTime(2025, 6, 28, 16, 0, 40, 438, DateTimeKind.Utc).AddTicks(1900), "ECommerce" }
                });

            migrationBuilder.InsertData(
                table: "LignesVente",
                columns: new[] { "LigneVenteId", "PrixUnitaire", "ProduitId", "Quantite", "VenteId" },
                values: new object[] { 1, 12.5m, 1, 2, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_LignesVente_VenteId",
                table: "LignesVente",
                column: "VenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesVente");

            migrationBuilder.DropTable(
                name: "Ventes");
        }
    }
}
