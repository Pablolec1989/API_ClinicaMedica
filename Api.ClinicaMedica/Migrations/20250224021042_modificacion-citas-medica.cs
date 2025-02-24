using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.ClinicaMedica.Migrations
{
    /// <inheritdoc />
    public partial class modificacioncitasmedica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas");

            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_Servicios_IdServicio",
                table: "CitasMedicas");

            migrationBuilder.DropIndex(
                name: "IX_CitasMedicas_IdMedico",
                table: "CitasMedicas");

            migrationBuilder.DropIndex(
                name: "IX_CitasMedicas_IdServicio",
                table: "CitasMedicas");

            migrationBuilder.DropColumn(
                name: "IdMedico",
                table: "CitasMedicas");

            migrationBuilder.DropColumn(
                name: "IdServicio",
                table: "CitasMedicas");

            migrationBuilder.AddColumn<string>(
                name: "IdMedico",
                table: "DetalleServicios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdPaciente",
                table: "DetalleServicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdPaciente",
                table: "CitasMedicas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServicios_IdMedico",
                table: "DetalleServicios",
                column: "IdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleServicios_Medicos_IdMedico",
                table: "DetalleServicios",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleServicios_Medicos_IdMedico",
                table: "DetalleServicios");

            migrationBuilder.DropIndex(
                name: "IX_DetalleServicios_IdMedico",
                table: "DetalleServicios");

            migrationBuilder.DropColumn(
                name: "IdMedico",
                table: "DetalleServicios");

            migrationBuilder.DropColumn(
                name: "IdPaciente",
                table: "DetalleServicios");

            migrationBuilder.AlterColumn<string>(
                name: "IdPaciente",
                table: "CitasMedicas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdMedico",
                table: "CitasMedicas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdServicio",
                table: "CitasMedicas",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_IdMedico",
                table: "CitasMedicas",
                column: "IdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_IdServicio",
                table: "CitasMedicas",
                column: "IdServicio");

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_Servicios_IdServicio",
                table: "CitasMedicas",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
