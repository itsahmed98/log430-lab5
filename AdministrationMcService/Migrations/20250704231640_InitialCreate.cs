using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdministrationMcService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Performances",
                columns: table => new
                {
                    PerformanceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MagasinId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChiffreAffaires = table.Column<decimal>(type: "numeric", nullable: false),
                    NbVentes = table.Column<int>(type: "integer", nullable: false),
                    RupturesStock = table.Column<int>(type: "integer", nullable: false),
                    Surstock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Performances", x => x.PerformanceId);
                });

            migrationBuilder.InsertData(
                table: "Performances",
                columns: new[] { "PerformanceId", "ChiffreAffaires", "Date", "MagasinId", "NbVentes", "RupturesStock", "Surstock" },
                values: new object[,]
                {
                    { 1, 1000m, new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc), 2, 50, 2, 5 },
                    { 2, 1500m, new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc), 3, 70, 1, 3 },
                    { 3, 1100m, new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc), 2, 55, 0, 4 },
                    { 4, 1600m, new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc), 3, 75, 2, 2 },
                    { 5, 1200m, new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), 2, 60, 1, 6 },
                    { 6, 1700m, new DateTime(2025, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), 3, 80, 0, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Performances");
        }
    }
}
