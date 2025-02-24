using Api.ClinicaMedica.Entities;

namespace Api.ClinicaMedica.DTO.Basic
{
    public class CitasMedicasDTO
    {
        public string IdCitas { get; set; }
        public string IdPaciente { get; set; }
        //public string IdPaquete { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime HoraConsulta { get; set; }
        public decimal MontoTotal { get; set; }
        public int PagadoONo { get; set; }

        // Relacion con DetalleServicio
        public Pacientes Paciente { get; set; } = null!;
       public ICollection<DetalleServiciosDTO> DetallesServicios { get; set; } = new List<DetalleServiciosDTO>();
    }
}
