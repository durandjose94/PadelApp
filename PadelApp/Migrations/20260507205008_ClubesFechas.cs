using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelApp.Migrations
{
    /// <inheritdoc />
    public partial class ClubesFechas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_actualizacion",
                table: "Clubes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_registro",
                table: "Clubes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_actualizacion",
                table: "Clubes");

            migrationBuilder.DropColumn(
                name: "fecha_registro",
                table: "Clubes");
        }
    }
}
