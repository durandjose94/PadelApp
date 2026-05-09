using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelApp.Migrations
{
    /// <inheritdoc />
    public partial class tablaInvitacionClub_modificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdRol",
                table: "InvitacionClubes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRol",
                table: "InvitacionClubes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
