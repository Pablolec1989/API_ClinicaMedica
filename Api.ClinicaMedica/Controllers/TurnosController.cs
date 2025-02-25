using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.ClinicaMedica.AccesoDatos;
using Api.ClinicaMedica.Entities;
using Api.ClinicaMedica.DTO.Create;

namespace Api.ClinicaMedica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TurnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Turnos/disponibles
        // Este endpoint devuelve solo los turnos que están disponibles
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerTurnosDisponibles()
        {
            var turnosDisponibles = await _context.Turnos
            .Where(t => t.Estado == "Disponible")  
            .Include(t => t.Horario)
            .Include(t => t.Medico)
            .OrderBy(t => t.Fecha)
            .ThenBy(t => t.Horario.HorarioInicio)  
            .ToListAsync();


            var turnosDTO = turnosDisponibles.Select(t => new TurnoDTO
            {
                IdTurno = t.IdTurno,
                IdHorario = t.IdHorario,
                IdMedico = t.IdMedico,
                Fecha = t.Fecha,
                Asistencia = t.Asistencia,
                IdPaciente = t.IdPaciente,
                Estado = t.Estado,
            }).ToList();

            return Ok(turnosDTO);
        }


        // GET: api/Turnos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerTurnoPorId(string id)

        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("El ID del turno no puede estar vacio");



            var turno = await _context.Turnos
                .Include(t => t.Horario)
                .Include(t => t.Medico)
                .Include(t => t.Paciente)
                .FirstOrDefaultAsync(t => t.IdTurno == id);

            if (turno == null)
                return NotFound($"No se encontró el turno con ID {id}");

            return Ok(turno);
        }

        // PUT: api/Turnos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTurno(string id, Turnos turnoActualizado)
        {
            if (string.IsNullOrWhiteSpace(id) || id != turnoActualizado.IdTurno)
                return BadRequest("El ID del turno no es válido o no coincide.");

            var turnoExistente = await _context.Turnos.FindAsync(id);
            if (turnoExistente == null)
                return NotFound($"No se encontró el turno con ID {id}");

            // Actualizar los valores
            turnoExistente.IdHorario = turnoActualizado.IdHorario;
            turnoExistente.IdMedico = turnoActualizado.IdMedico;
            turnoExistente.IdPaciente = turnoActualizado.IdPaciente;
            turnoExistente.Fecha = turnoActualizado.Fecha;
            turnoExistente.Estado = turnoActualizado.Estado;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurnosExists(id))
                    return NotFound();
                else
                    throw;
            }

            return Ok("Turno actualizado correctamente.");
        }

        // POST: api/Turnos/crear
        // Este endpoint es para que el administrador cree los turnos
        [HttpPost("crear")]
        public async Task<ActionResult<TurnoDTO>> CrearTurno([FromBody] TurnoDTO turnoDTO)
        {
            // Validar que el IdHorario existe en la base de datos
            var horario = await _context.Horarios.FindAsync(turnoDTO.IdHorario);
            if (horario == null)
                return BadRequest("El horario seleccionado no es válido.");

            bool turnoExistente = await _context.Turnos.AnyAsync(t => t.IdHorario == turnoDTO.IdHorario &&
                                                                      t.IdMedico == turnoDTO.IdMedico &&
                                                                      t.Fecha.Date == turnoDTO.Fecha.Date);

            if (turnoExistente)
                return BadRequest("Ya existe un turno asignado para este médico en esa franja horaria y fecha.");

            var turno = new Turnos
            {
                IdTurno = turnoDTO.IdTurno,
                IdHorario = turnoDTO.IdHorario,
                IdMedico = turnoDTO.IdMedico,
                Fecha = turnoDTO.Fecha,
                Asistencia = turnoDTO.Asistencia, //por defecto
                IdPaciente = turnoDTO.IdPaciente,
                Estado = "Disponible"
            };

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();

            return Ok(turnoDTO);
        }

        // POST: api/Turnos/reservar
        // Este endpoint permite que un paciente reserve un turno
        [HttpPost("reservar")]
        public async Task<IActionResult> ReservarTurno([FromBody] ReservaTurnoDTO reserva)
        {
            var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.IdTurno == reserva.IdTurno);

            if (turno == null)
                return NotFound("El turno no existe.");

            if (turno.Estado == "Ocupado")
                return BadRequest("El turno ya está ocupado.");

            turno.Estado = "Ocupado"; 
            turno.IdPaciente = reserva.IdPaciente; 

            _context.Turnos.Update(turno);
            await _context.SaveChangesAsync();

            return Ok("Turno reservado con éxito.");
        }

        [HttpPost("cancelar")]
        public async Task<IActionResult> CancelarTurno([FromBody] CancelarTurnoDTO cancelacion)
        {
            var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.IdTurno == cancelacion.IdTurno);

            if (turno == null)
                return NotFound("El turno no existe.");

            if (turno.Estado == "Disponible")
                return BadRequest("El turno ya está disponible, no necesita ser cancelado.");

            turno.Estado = "Disponible"; 
            turno.IdPaciente = null; 

            await _context.SaveChangesAsync();
            return Ok("Turno cancelado y ahora está disponible.");
        }


        // DELETE: api/Turnos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTurno(string id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
                return NotFound($"No se encontró el turno con ID {id}");

            try
            {
                _context.Turnos.Remove(turno);
                await _context.SaveChangesAsync();
                return Ok("Turno eliminado correctamente.");
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el turno porque tiene dependencias.");
            }
        }

        private bool TurnosExists(string id)
        {
            return _context.Turnos.Any(e => e.IdTurno == id);
        }
    }
}
