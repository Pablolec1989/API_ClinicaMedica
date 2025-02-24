using Api.ClinicaMedica.Entities;
using System.ComponentModel.DataAnnotations;

namespace Api.ClinicaMedica.DTO.Create
{
    public class CitasMedicasCreacionDTO
    {
        [Required]
        public string IdCitas { get; set; }
        [Required]
        public string IdPaciente { get; set; }
        public DateTime? FechaConsulta { get; set; }
        public DateTime? HoraConsulta { get; set; }
        public decimal MontoTotal { get; set; }
        public int PagadoONo { get; set; }

        public ICollection<DetalleServicioCreacionDTO> DetallesServicios { get; set; } = new List<DetalleServicioCreacionDTO>();
    }
}
