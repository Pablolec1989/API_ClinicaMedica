namespace Api.ClinicaMedica.Entities
{
    public class CitasMedicas
    {
        public string IdCitas { get; set; }
        public string? IdPaciente { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime HoraConsulta { get; set; }
        public decimal MontoTotal { get; set; }
        public int PagadoONo { get; set; }

        // Relación con Paciente
        public Pacientes? Paciente { get; set; } = null!;

        // Relación con DetalleServicios
        public ICollection<DetalleServicios> DetallesServicios { get; set; } = new List<DetalleServicios>();
    }
}
