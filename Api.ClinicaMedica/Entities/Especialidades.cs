namespace Api.ClinicaMedica.Entities
{
    public class Especialidades
    {
        public string IdEspecialidad { get; set; }
        public string Detalle { get; set; }

        public ICollection<Medicos> Medicos { get; set; } = new List<Medicos>();
    }
}
