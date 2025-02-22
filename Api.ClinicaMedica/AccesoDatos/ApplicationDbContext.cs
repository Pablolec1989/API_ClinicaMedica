using Api.ClinicaMedica.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Api.ClinicaMedica.AccesoDatos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet
        public DbSet<Pacientes> Pacientes { get; set; }
        public DbSet<Medicos> Medicos { get; set; }
        public DbSet<Especialidades> Especialidades { get; set; }
        public DbSet<Horarios> Horarios { get; set; }
        public DbSet<Turnos> Turnos { get; set; }
        public DbSet<Roles> Roles { get; set; }

        public DbSet<ServiciosDTO> Servicios { get; set; }

        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ---------- Medicos ----------------

            modelBuilder.Entity<Medicos>()
                .HasKey(m => m.IdMedico);

            modelBuilder.Entity<Medicos>()
                .HasOne(m => m.Especialidad)
                .WithMany(e => e.Medicos)
                .HasForeignKey(m => m.IdEspecialidad)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            modelBuilder.Entity<Medicos>()
                .HasMany(m => m.Turnos)
                .WithOne(t => t.Medico)
                .HasForeignKey(t => t.IdMedico)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra un médico, los turnos quedan huérfanos

            modelBuilder.Entity<Medicos>()
                    .HasMany(m => m.Turnos)
                    .WithOne(t => t.Medico) // Asegúrate de que `Turnos` tenga una propiedad `Medico`
                    .HasForeignKey(t => t.IdMedico)
                    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Medicos>(entity =>
            {
                entity.Property(e => e.Sueldo)
                      .HasColumnType("decimal(18,2)"); // Especifica la precisión y escala
            });

            // -------- Pacientes ----------------

            modelBuilder.Entity<Pacientes>()
                .HasKey(p => p.IdPaciente);

            modelBuilder.Entity<Pacientes>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Paciente)
                .HasForeignKey(t => t.IdPaciente)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra un paciente, los turnos quedan sin paciente

            // -------- Especialidades ------------

            modelBuilder.Entity<Especialidades>()
                .HasKey(e => e.IdEspecialidad); // Define la clave primaria explícitamente

            modelBuilder.Entity<Especialidades>()
                .HasMany(e => e.Medicos)
                .WithOne(m => m.Especialidad)
                .HasForeignKey(m => m.IdEspecialidad)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            // Agregado para el segundo commit, Config para Personas, Horarios Turnos
            // 📌 Horarios
            modelBuilder.Entity<Horarios>()
                .HasKey(h => h.IdHorario);

            modelBuilder.Entity<Horarios>()
                .HasMany(h => h.Turnos)
                .WithOne(t => t.Horario)
                .HasForeignKey(t => t.IdHorario)
                .OnDelete(DeleteBehavior.Restrict); // Si se borra un horario, se eliminan los turnos

            // Sembrar franjas horarias de 30 minutos
            modelBuilder.Entity<Horarios>().HasData(
                // Turnos de la mañana: 08:00 - 12:00
                new Horarios { IdHorario = "H1", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(8, 30, 0) },
                new Horarios { IdHorario = "H2", HorarioInicio = new TimeSpan(8, 30, 0), HorarioFin = new TimeSpan(9, 0, 0) },
                new Horarios { IdHorario = "H3", HorarioInicio = new TimeSpan(9, 0, 0), HorarioFin = new TimeSpan(9, 30, 0) },
                new Horarios { IdHorario = "H4", HorarioInicio = new TimeSpan(9, 30, 0), HorarioFin = new TimeSpan(10, 0, 0) },
                new Horarios { IdHorario = "H5", HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(10, 30, 0) },
                new Horarios { IdHorario = "H6", HorarioInicio = new TimeSpan(10, 30, 0), HorarioFin = new TimeSpan(11, 0, 0) },
                new Horarios { IdHorario = "H7", HorarioInicio = new TimeSpan(11, 0, 0), HorarioFin = new TimeSpan(11, 30, 0) },
                new Horarios { IdHorario = "H8", HorarioInicio = new TimeSpan(11, 30, 0), HorarioFin = new TimeSpan(12, 0, 0) },

                // Turnos de la tarde: 13:00 - 17:00
                new Horarios { IdHorario = "H9", HorarioInicio = new TimeSpan(13, 0, 0), HorarioFin = new TimeSpan(13, 30, 0) },
                new Horarios { IdHorario = "H10", HorarioInicio = new TimeSpan(13, 30, 0), HorarioFin = new TimeSpan(14, 0, 0) },
                new Horarios { IdHorario = "H11", HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(14, 30, 0) },
                new Horarios { IdHorario = "H12", HorarioInicio = new TimeSpan(14, 30, 0), HorarioFin = new TimeSpan(15, 0, 0) },
                new Horarios { IdHorario = "H13", HorarioInicio = new TimeSpan(15, 0, 0), HorarioFin = new TimeSpan(15, 30, 0) },
                new Horarios { IdHorario = "H14", HorarioInicio = new TimeSpan(15, 30, 0), HorarioFin = new TimeSpan(16, 0, 0) },
                new Horarios { IdHorario = "H15", HorarioInicio = new TimeSpan(16, 0, 0), HorarioFin = new TimeSpan(16, 30, 0) },
                new Horarios { IdHorario = "H16", HorarioInicio = new TimeSpan(16, 30, 0), HorarioFin = new TimeSpan(17, 0, 0) }
            );

            // ----- 📌 Turnos ---------------

            modelBuilder.Entity<Turnos>(entity =>
            {
                entity.HasKey(t => t.IdTurno);

                entity.Property(t => t.Fecha)
                .HasColumnType("date")
                .IsRequired();

                entity.Property(t => t.Estado)
                      .HasMaxLength(20)
                      .HasDefaultValue("Disponible");

                // Configurar IdPaciente como clave foránea nullable
                entity.HasOne(t => t.Paciente)
                    .WithMany(p => p.Turnos)    //Usa la colection de Turnos en Paciente
                    .HasForeignKey(t => t.IdPaciente)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(t => t.Horario)
                    .WithMany(h => h.Turnos)
                    .HasForeignKey(t => t.IdHorario)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacion con Medicos
                entity.HasOne(t => t.Medico)
                    .WithMany(h => h.Turnos)
                    .HasForeignKey(t => t.IdMedico)
                    .OnDelete(DeleteBehavior.Restrict);

                // Agregar restricción UNIQUE en HorarioId, MedicoId y Fecha
                entity.HasIndex(t => new { t.IdHorario, t.IdMedico, t.Fecha })
                    .IsUnique()
                    .HasDatabaseName("UQ_Turnos_Horario_Medico_Fecha");
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                // Definir clave primaria
                entity.HasKey(u => u.IdUsuario);

                // Configurar propiedades
                entity.Property(u => u.IdUsuario)
                    .IsRequired()
                    .HasMaxLength(50); // Ajusta el tamaño según sea necesario

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Dni)
                    .HasMaxLength(20); // Puede ser NULL según la entidad

                entity.Property(u => u.Email)
                    .HasMaxLength(150);

                entity.Property(u => u.FechaNacimiento)
                    .HasColumnType("date"); // Opcional: Mejor definición en base de datos

                entity.Property(u => u.Telefono)
                    .HasMaxLength(20);

                entity.Property(u => u.Direccion)
                    .HasMaxLength(200);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(255); // Tamaño recomendado para almacenar hash

                // Relación con Rol (suponiendo que existe una tabla de Roles)
                entity.HasOne<Roles>()  // Cambia "Rol" por la entidad correspondiente
                    .WithMany()
                    .HasForeignKey(u => u.IdRol)
                    .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

                // Nombre de la tabla en la BD (opcional)
                entity.ToTable("Usuarios");
            });

            // Configuración de la entidad Roles
            modelBuilder.Entity<Roles>(entity =>
        {
            entity.ToTable("Roles");

            entity.Property(r => r.IdRol)
                .ValueGeneratedNever(); // Evita que se genere automáticamente (útil si insertas datos iniciales)

            entity.Property(r => r.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            // Seed de datos iniciales (opcional)
            entity.HasData(
                new Roles { IdRol = 1, Nombre = "Administrador" },
                new Roles { IdRol = 2, Nombre = "Médico" },
                new Roles { IdRol = 3, Nombre = "Paciente" }
            );

            modelBuilder.Entity<ServiciosDTO>(entity =>
            {
                entity.HasKey(s => s.IdServicio);
                entity.Property(s => s.IdServicio)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(s => s.Nombre)
                      .IsRequired()
                      .HasMaxLength(100); 

                entity.Property(s => s.Descripcion)
                      .HasMaxLength(700); 

                entity.Property(s => s.Precio)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
            });

        });

        }
    }
}
