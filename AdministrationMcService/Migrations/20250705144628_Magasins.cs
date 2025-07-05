using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdministrationMcService.Migrations
{
    /// <inheritdoc />
    public partial class Magasins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Magasins",
                columns: table => new
                {
                    MagasinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Adresse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magasins", x => x.MagasinId);
                });

            migrationBuilder.InsertData(
                table: "Magasins",
                columns: new[] { "MagasinId", "Adresse", "Nom" },
                values: new object[,]
                {
                    { 1, "123 Rue entrepot", "Entrepot Central" },
                    { 2, "123 Rue Principale", "Magasin A" },
                    { 3, "456 Avenue Centrale", "Magasin B" },
                    { 4, "789 Boulevard Sud", "Magasin C" },
                    { 5, "321 Rue Nord", "Magasin D" }
                });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 5,
                column: "Date",
                value: new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 6,
                column: "Date",
                value: new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Magasins");

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 5,
                column: "Date",
                value: new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 6,
                column: "Date",
                value: new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
