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

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<CitasMedicas> CitasMedicas { get; set; }
        public DbSet<DetalleServicios> DetalleServicios {  get; set; }

        public DbSet<Paquetes> Paquetes { get; set; }
        public DbSet<PaqueteServicio> PaqueteServicios { get; set; }
        public DbSet<Consultas> Consultas { get; set; }
        public DbSet<Facturacion> Facturaciones { get; set; }


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
            });

            // Servicios
            modelBuilder.Entity<Servicios>(entity =>
            {
                // Nombre de la tabla
                entity.ToTable("Servicios");

                // Clave primaria
                entity.HasKey(s => s.IdServicio);

                // Configuración de propiedades
                entity.Property(s => s.IdServicio)
                    .HasMaxLength(50)  // Longitud máxima del ID
                    .IsRequired();     // No puede ser nulo

                entity.Property(s => s.Nombre)
                    .HasMaxLength(100) // Longitud máxima del nombre
                    .IsRequired();     // No puede ser nulo

                entity.Property(s => s.Descripcion)
                    .HasMaxLength(500); // Longitud máxima de la descripción

                entity.Property(s => s.Precio)
                    .HasColumnType("decimal(18,2)") // Define la precisión del decimal
                    .IsRequired(); // No puede ser nulo
            });

            // *** CITAS MÉDICAS ***
            modelBuilder.Entity<CitasMedicas>(entity =>
            {
                entity.HasKey(c => c.IdCitas);

                entity.HasOne(c => c.Paciente)
                      .WithMany()
                      .HasForeignKey(c => c.IdPaciente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Medico)
                      .WithMany()
                        .HasForeignKey(c => c.IdMedico)
                        .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.Servicio)
                      .WithMany()
                      .HasForeignKey(c => c.IdServicio)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.DetallesServicios)
                      .WithOne(ds => ds.CitaMedica)
                      .HasForeignKey(ds => ds.IdCitas)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.MontoTotal)
                       .HasPrecision(18, 2); // Define la precisión del decimal
            });

            // *** DETALLE SERVICIOS (Clave compuesta) ***
            modelBuilder.Entity<DetalleServicios>(entity =>
            {
                entity.HasKey(ds => new { ds.IdCitas, ds.IdServicio }); // Clave compuesta

                entity.Property(ds => ds.MontoParcial)
                        .HasPrecision(18, 2); // Define la precisión del decimal

                entity.HasOne(ds => ds.CitaMedica)
                      .WithMany(c => c.DetallesServicios)
                      .HasForeignKey(ds => ds.IdCitas)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ds => ds.Servicio)
                      .WithMany()
                      .HasForeignKey(ds => ds.IdServicio)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaqueteServicio>()
            .HasOne(ps => ps.Paquete)
            .WithMany(p => p.PaqueteServicios)
            .HasForeignKey(ps => ps.CodigoPaquete)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaqueteServicio>()
                .HasOne(ps => ps.Servicio)
                .WithMany()
                .HasForeignKey(ps => ps.CodigoServicio)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Consultas>()
            .HasOne(c => c.Servicio)
            .WithMany()
            .HasForeignKey(c => c.IdServicio)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Consultas>()
                .HasOne(c => c.Paquete)
                .WithMany()
                .HasForeignKey(c => c.IdPaquete)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Consultas>(entity =>
            {
                entity.HasKey(c => c.IdConsulta);

                entity.Property(c => c.FechaConsulta)
                      .IsRequired();

                entity.Property(c => c.HoraConsulta)
                      .IsRequired();

                entity.Property(c => c.MontoTotal)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(c => c.Pagado)
                      .IsRequired();

                // Clave foránea opcional (nullable) para Paciente
                entity.HasOne(c => c.Paciente)
                      .WithMany() // Si Paciente tiene una lista de Consultas, usa `.WithMany(p => p.Consultas)`
                      .HasForeignKey(c => c.IdPaciente)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull); // Opcional: si se borra el paciente, se pone en NULL

                // Clave foránea opcional (nullable) para Medico
                entity.HasOne(c => c.Medico)
                      .WithMany()
                      .HasForeignKey(c => c.IdMedico)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                // Clave foránea opcional (nullable) para Servicio
                entity.HasOne(c => c.Servicio)
                      .WithMany()
                      .HasForeignKey(c => c.IdServicio)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                // Clave foránea opcional (nullable) para Paquete
                entity.HasOne(c => c.Paquete)
                      .WithMany()
                      .HasForeignKey(c => c.IdPaquete)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            
            modelBuilder.Entity<Facturacion>()
                .Property(f => f.FechaPago)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Facturacion>()
            .Property(f => f.MontoPagado)
            .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Paquetes>()
            .Property(p => p.PrecioPaquete)
            .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Facturacion>()
                .HasOne(f => f.Consulta)  // Relación 1 a 1 o 1 a muchos
                .WithMany()  // Si `Consultas` no tiene lista de `Facturacion`, usa WithMany()
                .HasForeignKey(f => f.IdConsulta)  // Clave foránea
                .OnDelete(DeleteBehavior.Restrict);  // Evita eliminaciones en cascada si no lo deseas

        }
    }
}
