using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelApp.Migrations
{
    /// <inheritdoc />
    public partial class tablaAnuncio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anuncios",
                columns: table => new
                {
                    idAnuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    tipoAnuncio = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nivelRequerido = table.Column<decimal>(type: "decimal(2,1)", nullable: true),
                    fechaEvento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    telefonoContacto = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    permiteWhatsapp = table.Column<bool>(type: "bit", nullable: false),
                    permiteLlamada = table.Column<bool>(type: "bit", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anuncios", x => x.idAnuncio);
                    table.ForeignKey(
                        name: "FK_Anuncios_Usuarios_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anuncios_idUsuario",
                table: "Anuncios",
                column: "idUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anuncios");
        }
    }
}
