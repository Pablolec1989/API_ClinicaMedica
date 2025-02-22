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
using Newtonsoft.Json.Linq;

namespace Api.ClinicaMedica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiciosDTO>>> GetServicios()
        {
           var servicios = await _context.Servicios
                  .Select(s => new ServicioDTO
                  {
                      IdServicio = s.IdServicio,
                      Nombre = s.Nombre,
                      Descripcion = s.Descripcion,
                      Precio = s.Precio
                  })
                .ToListAsync();
             return Ok(servicios);

        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiciosDTO>> GetServicios(string id)
        {
            var servicios = await _context.Servicios.FindAsync(id);

            if (servicios == null)
            {
                return NotFound();
            }

            var servicioDTO = new ServicioDTO 
            { 
              IdServicio= servicios.IdServicio,
              Nombre= servicios.Nombre,
              Descripcion= servicios.Descripcion,
              Precio= servicios.Precio
            };
            return Ok(servicioDTO);
        }

        // PUT: api/Servicios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicios(string id, ServiciosDTO servicios)
        {
            if (id != servicios.IdServicio)
            {
                return BadRequest();
            }

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            // Actualizar la entidad con los valores del DTO
            servicio.Nombre = servicios.Nombre;
            servicio.Descripcion = servicios.Descripcion;
            servicio.Precio = servicios.Precio;

            _context.Entry(servicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiciosExists(id))
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

        // POST: api/Servicios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServiciosDTO>> PostServicios(ServiciosDTO servicios)
        {
            var servicio = new ServiciosDTO
            {
                IdServicio = servicios.IdServicio,
                Nombre = servicios.Nombre,
                Descripcion = servicios.Descripcion,
                Precio = servicios.Precio,

            };

            _context.Servicios.Add(servicio);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServiciosExists(servicio.IdServicio))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Devolver el DTO creado
            var resultDTO = new ServiciosDTO
            {
                IdServicio = servicios.IdServicio,
                Nombre = servicios.Nombre,
                Descripcion = servicios.Descripcion,
                Precio = servicios.Precio
            };

            return CreatedAtAction("GetServicios", new { id = servicio.IdServicio }, resultDTO);
        }
        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicios(string id)
        {
            var servicios = await _context.Servicios.FindAsync(id);

            if (servicios == null)
            {
                return NotFound();
            }

            _context.Servicios.Remove(servicios);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiciosExists(string id)
        {
            return _context.Servicios.Any(e => e.IdServicio == id);
        }
    }
}
