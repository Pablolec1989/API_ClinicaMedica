using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.ClinicaMedica.Migrations
{
    /// <inheritdoc />
    public partial class cambioforanea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas");

            migrationBuilder.AlterColumn<string>(
                name: "IdMedico",
                table: "CitasMedicas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas");

            migrationBuilder.AlterColumn<string>(
                name: "IdMedico",
                table: "CitasMedicas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_Medicos_IdMedico",
                table: "CitasMedicas",
                column: "IdMedico",
                principalTable: "Medicos",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
