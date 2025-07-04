using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class PreviousVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "TypeVente",
                table: "Ventes");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnLigne",
                table: "Ventes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "LignesVente",
                keyColumn: "LigneVenteId",
                keyValue: 1,
                column: "PrixUnitaire",
                value: 1.50m);

            migrationBuilder.InsertData(
                table: "LignesVente",
                columns: new[] { "LigneVenteId", "PrixUnitaire", "ProduitId", "Quantite", "VenteId" },
                values: new object[,]
                {
                    { 2, 3.75m, 2, 1, 1 },
                    { 3, 12.00m, 3, 5, 2 }
                });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                columns: new[] { "ClientId", "Date", "IsEnLigne", "MagasinId" },
                values: new object[] { null, new DateTime(2025, 7, 2, 18, 31, 41, 34, DateTimeKind.Utc).AddTicks(4815), false, 2 });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                columns: new[] { "ClientId", "Date", "IsEnLigne", "MagasinId" },
                values: new object[] { null, new DateTime(2025, 7, 3, 18, 31, 41, 34, DateTimeKind.Utc).AddTicks(4822), false, 3 });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 3,
                columns: new[] { "ClientId", "Date", "IsEnLigne", "MagasinId" },
                values: new object[] { 2, new DateTime(2025, 7, 1, 18, 31, 41, 34, DateTimeKind.Utc).AddTicks(4824), true, null });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 4,
                columns: new[] { "ClientId", "Date", "IsEnLigne", "MagasinId" },
                values: new object[] { null, new DateTime(2025, 6, 29, 18, 31, 41, 34, DateTimeKind.Utc).AddTicks(4826), false, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LignesVente",
                keyColumn: "LigneVenteId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LignesVente",
                keyColumn: "LigneVenteId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "IsEnLigne",
                table: "Ventes");

            migrationBuilder.AddColumn<string>(
                name: "TypeVente",
                table: "Ventes",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "LignesVente",
                keyColumn: "LigneVenteId",
                keyValue: 1,
                column: "PrixUnitaire",
                value: 12.5m);

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
        }
    }
}
