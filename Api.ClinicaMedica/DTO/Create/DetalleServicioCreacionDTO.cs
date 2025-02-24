using System.ComponentModel.DataAnnotations;

namespace Api.ClinicaMedica.DTO.Create
{
    public class DetalleServicioCreacionDTO
    {
        [Required]
        public string IdCitas { get; set; }
        [Required]
        public string IdServicio { get; set; }
        public string IdPaciente { get; set; }
        public string? IdMedico { get; set; } // Se usa aquí en lugar de CitasMedicas

        public decimal? MontoParcial { get; set; }
    }
}
