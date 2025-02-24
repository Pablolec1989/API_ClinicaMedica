namespace Api.ClinicaMedica.Entities
{
    public class DetalleServicios
    {
        public string IdCitas { get; set; }
        public string IdPaciente { get; set; }
        public string IdServicio { get; set; }
        public string? IdMedico { get; set; } // Se usa aquí en lugar de CitasMedicas
        public decimal MontoParcial { get; set; }

        // Propiedades de navegación
        public CitasMedicas CitaMedica { get; set; }
        public Servicios Servicio { get; set; }
        public Medicos Medico { get; set; } // Se agrega relación con Medico
    }
}
