using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Botica.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRecuperacionContrasenaYEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoRecuperacion",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CodigoRecuperacionExpira",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpContrasena",
                table: "ConfiguracionSistema",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmtpHost",
                table: "ConfiguracionSistema",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmtpPuerto",
                table: "ConfiguracionSistema",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SmtpRemitente",
                table: "ConfiguracionSistema",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SmtpUsarSsl",
                table: "ConfiguracionSistema",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SmtpUsuario",
                table: "ConfiguracionSistema",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoRecuperacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CodigoRecuperacionExpira",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SmtpContrasena",
                table: "ConfiguracionSistema");

            migrationBuilder.DropColumn(
                name: "SmtpHost",
                table: "ConfiguracionSistema");

            migrationBuilder.DropColumn(
                name: "SmtpPuerto",
                table: "ConfiguracionSistema");

            migrationBuilder.DropColumn(
                name: "SmtpRemitente",
                table: "ConfiguracionSistema");

            migrationBuilder.DropColumn(
                name: "SmtpUsarSsl",
                table: "ConfiguracionSistema");

            migrationBuilder.DropColumn(
                name: "SmtpUsuario",
                table: "ConfiguracionSistema");
        }
    }
}
