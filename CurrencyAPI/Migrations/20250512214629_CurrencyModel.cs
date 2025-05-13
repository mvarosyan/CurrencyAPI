using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CurrencyAPI.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "CurrencyRates");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "CurrencyRates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_CurrencyId",
                table: "CurrencyRates",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyRates_Currencies_CurrencyId",
                table: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyRates_CurrencyId",
                table: "CurrencyRates");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CurrencyRates");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "CurrencyRates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
