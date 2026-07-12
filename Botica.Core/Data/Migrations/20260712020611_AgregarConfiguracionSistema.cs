using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Botica.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionSistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreEmpresa = table.Column<string>(type: "TEXT", nullable: false),
                    Ruc = table.Column<string>(type: "TEXT", nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    TasaIgv = table.Column<decimal>(type: "TEXT", precision: 5, scale: 4, nullable: false),
                    Moneda = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSistema", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionSistema");
        }
    }
}
