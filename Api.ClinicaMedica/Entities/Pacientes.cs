namespace Api.ClinicaMedica.Entities
{
    public class Pacientes
    {
        public string IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string Password { get; set; } = null!;
        public int IdRol { get; set; }
        public bool ObraSocial { get; set; }
        
        public virtual ICollection<Turnos> Turnos { get; set; } = new List<Turnos>();
    }
}
