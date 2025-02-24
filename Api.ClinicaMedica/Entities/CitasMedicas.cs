namespace Api.ClinicaMedica.Entities
{
    public class CitasMedicas
    {
        public string IdCitas { get; set; }
        public string? IdMedico { get; set; }
        public string IdPaciente { get; set; }
        public string IdServicio { get; set; }
        //public string IdPaquete { get; set; }
        public DateTime FechaConsulta { get; set; }
        public DateTime HoraConsulta { get; set; }
        public decimal MontoTotal { get; set; }
        public int PagadoONo { get; set; }

        // Relacion con DetalleServicio
        public Pacientes Paciente { get; set; } = null!;
        public Medicos? Medico { get; set; } = null!;
        public Servicios Servicio { get; set; } = null!;
        public ICollection<DetalleServicios> DetallesServicios { get; set; } = new List<DetalleServicios>();
    }
}
