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
using Api.ClinicaMedica.DTO.Put;

namespace Api.ClinicaMedica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MedicosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Medicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicos>>> GetMedicos()
        {
            return await _context.Medicos.ToListAsync();
        }

        // GET: api/Medicos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicos>> GetMedicos(string id)
        {
            var medicos = await _context.Medicos.FindAsync(id);

            if (medicos == null)
            {
                return NotFound();
            }

            return medicos;
        }

        // PUT: api/Medicos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicos(string id, MedicosPutDTO medicosDTO)
        {
            if (id != medicosDTO.IdMedico)
            {
                return BadRequest();
            }
            var medico = _context.Medicos.Find(id);

            if (medico != null)
            {
                medico.IdMedico = medicosDTO.IdMedico;
                medico.Nombre = medicosDTO.Nombre;
                medico.Apellido = medicosDTO.Apellido;
                medico.Dni = medicosDTO.Dni;
                medico.Email = medicosDTO.Email;
                medico.Telefono = medicosDTO.Telefono;
                medico.IdEspecialidad = medicosDTO.IdEspecialidad;
                medico.Sueldo = medicosDTO.Sueldo;

                _context.Entry(medico).State = EntityState.Modified;
            }
            else
                return NotFound("Medico no encontrado");


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicosExists(id))
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

        // DELETE: api/Medicos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicos(string id)
        {
            var medicos = await _context.Medicos.FindAsync(id);
            if (medicos == null)
            {
                return NotFound();
            }

            _context.Medicos.Remove(medicos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicosExists(string id)
        {
            return _context.Medicos.Any(e => e.IdMedico == id);
        }
    }
}
