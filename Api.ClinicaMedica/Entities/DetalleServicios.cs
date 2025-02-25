namespace Api.ClinicaMedica.Entities
{
    public class DetalleServicios
    {
        public string IdCitas { get; set; }
        public string IdServicio { get; set; }
        public decimal MontoParcial { get; set; }

        public CitasMedicas CitaMedica { get; set; }
        public Servicios Servicio { get; set; }
    }
}
