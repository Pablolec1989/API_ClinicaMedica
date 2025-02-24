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
using AutoMapper;
using Api.ClinicaMedica.DTO.Basic;

namespace Api.ClinicaMedica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConsultasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Consultas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consultas>>> GetConsultas()
        {
            var ListaConsultas = await _context.Consultas.Include(c => c.Servicio).Include(c => c.Paciente)
                .Include(c => c.Paquete).Include(c => c.Medico).ToListAsync();

            var listaDTO = _mapper.Map<List<ConsultasDTO>>(ListaConsultas);
            return await _context.Consultas.ToListAsync();
        }

        // GET: api/Consultas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Consultas>> GetConsultas(string id)
        {
            var consultas = await _context.Consultas.Include(c => c.Servicio).Include(c => c.Paciente)
                        .Include(c => c.Paquete).Include(c => c.Medico).FirstOrDefaultAsync(c => c.IdConsulta == id);

            if (consultas == null)
            {
                return NotFound();
            }

            return consultas;
        }

        // PUT: api/Consultas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsultas(string id, ConsultasCreateDTO consultasDTO)
        {
            var consultas = _mapper.Map<Consultas>(consultasDTO);
            if (id != consultas.IdConsulta)
            {
                return BadRequest();
            }

            _context.Entry(consultas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Consultas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Consultas>> PostConsultas(ConsultasCreateDTO consultasDTO)
        {
            var consultas = _mapper.Map<Consultas>(consultasDTO);
            _context.Consultas.Add(consultas);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ConsultasExists(consultas.IdConsulta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetConsultas", new { id = consultas.IdConsulta }, consultas);
        }

        // DELETE: api/Consultas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsultas(string id)
        {
            var consultas = await _context.Consultas.FindAsync(id);
            if (consultas == null)
            {
                return NotFound();
            }

            _context.Consultas.Remove(consultas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConsultasExists(string id)
        {
            return _context.Consultas.Any(e => e.IdConsulta == id);
        }
    }
}
