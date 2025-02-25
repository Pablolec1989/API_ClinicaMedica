using Api.ClinicaMedica.AccesoDatos;
using Api.ClinicaMedica.DTO.Basic;
using Api.ClinicaMedica.DTO.Create;
using Api.ClinicaMedica.Entities;
using Api.ClinicaMedica.Models;
using Api.ClinicaMedica.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.ClinicaMedica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _confi;

        public RegisterController(ApplicationDbContext context, IMapper mapper, IConfiguration confi)
        {
            _context = context;
            _mapper = mapper;
            _confi = confi;
        }

        [HttpPost("register-paciente")]
        public async Task<ActionResult> RegisterPaciente([FromBody] PacientesCreateDTO pacientesCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            try
            {
                bool emailExists = await _context.Pacientes.AnyAsync(u => u.Email == pacientesCreateDTO.Email);
                if (emailExists)
                {
                    return BadRequest("El correo ya está registrado.");
                }

                var rol = await _context.Roles.FindAsync(pacientesCreateDTO.IdRol);
                if (rol == null)
                {
                    return BadRequest("El rol especificado no existe.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(pacientesCreateDTO.Password);

                var paciente = new Pacientes
                {
                    IdPaciente = pacientesCreateDTO.IdPaciente,
                    Nombre = pacientesCreateDTO.Nombre,
                    Apellido = pacientesCreateDTO.Apellido,
                    Dni = pacientesCreateDTO.Dni,
                    Email = pacientesCreateDTO.Email,
                    FechaNacimiento = pacientesCreateDTO.FechaNacimiento,
                    Telefono = pacientesCreateDTO.Telefono,
                    Direccion = pacientesCreateDTO.Direccion,
                    Password = hashedPassword,
                    IdRol = pacientesCreateDTO.IdRol,
                    ObraSocial = pacientesCreateDTO.ObraSocial
                };

                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                return Ok("Paciente registrado con éxito.");

            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                return StatusCode(500, $"Error al guardar en la base de datos: {innerException}");
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado. " + ex.Message);
            }
        }

        [HttpPost("register-medico")]
        public async Task<ActionResult> RegisterMedico([FromBody] MedicosCreateDTO medicosCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            try
            {
                bool emailExists = await _context.Medicos.AnyAsync(u => u.Email == medicosCreateDTO.Email);
                if (emailExists)
                {
                    return BadRequest("El correo ya está registrado.");
                }

                var rol = await _context.Roles.FindAsync(medicosCreateDTO.IdRol);
                if (rol == null)
                {
                    return BadRequest("El rol especificado no existe.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(medicosCreateDTO.Password);

                var medico = new Medicos
                {
                    IdMedico = medicosCreateDTO.IdMedico,
                    Nombre = medicosCreateDTO.Nombre,
                    Apellido = medicosCreateDTO.Apellido,
                    Dni = medicosCreateDTO.Dni,
                    Email = medicosCreateDTO.Email,
                    FechaNacimiento = medicosCreateDTO.FechaNacimiento,
                    Telefono = medicosCreateDTO.Telefono,
                    Direccion = medicosCreateDTO.Direccion,
                    IdRol = medicosCreateDTO.IdRol,
                    IdEspecialidad = medicosCreateDTO.IdEspecialidad,
                    Password = hashedPassword,
                    Sueldo = medicosCreateDTO.Sueldo
                };

                _context.Medicos.Add(medico);
                await _context.SaveChangesAsync();

                return Ok("Medico registrado con éxito.");

            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                return StatusCode(500, $"Error al guardar en la base de datos: {innerException}");
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado. " + ex.Message);
            }
        }

        [HttpPost("register-usuario")]
        public async Task<ActionResult> RegisterUsuario([FromBody] UsuariosCreateDTO usuariosCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            try
            {
                bool emailExists = await _context.Usuarios.AnyAsync(u => u.Email == usuariosCreateDTO.Email);
                if (emailExists)
                {
                    return BadRequest("El correo ya está registrado.");
                }

                var rol = await _context.Roles.FindAsync(usuariosCreateDTO.IdRol);
                if (rol == null)
                {
                    return BadRequest("El rol especificado no existe.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuariosCreateDTO.Password);

                // Crear un nuevo usuario
                var usuario = new Usuarios
                {
                    IdUsuario = usuariosCreateDTO.IdUsuario,
                    Nombre = usuariosCreateDTO.Nombre,
                    Apellido = usuariosCreateDTO.Apellido,
                    Dni = usuariosCreateDTO.Dni,
                    Email = usuariosCreateDTO.Email,
                    FechaNacimiento = usuariosCreateDTO.FechaNacimiento,
                    Telefono = usuariosCreateDTO.Telefono,
                    Direccion = usuariosCreateDTO.Direccion,
                    IdRol = usuariosCreateDTO.IdRol,
                    Password = hashedPassword,
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return Ok("Usuario registrado con éxito.");

            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                return StatusCode(500, $"Error al guardar en la base de datos: {innerException}");
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado. " + ex.Message);
            }
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacientesDTO>>> GetPacientes()
        {
            var listaPersonas = await _context.Pacientes.ToListAsync();
            return _mapper.Map<List<PacientesDTO>>(listaPersonas);
        }

        [HttpGet("obtener-medicos")]
        public async Task<ActionResult<IEnumerable<MedicosDTO>>> GetMedicos()
        {
            var listaMedicos = await _context.Pacientes.ToListAsync();
            return _mapper.Map<List<MedicosDTO>>(listaMedicos);
        }
            

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            object usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            string rol = "Administrador";

            if (usuario == null)
            {
                usuario = await _context.Medicos.FirstOrDefaultAsync(m => m.Email == loginDTO.Email);
                rol = "Médico";
            }
            if (usuario == null)
            {
                usuario = await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == loginDTO.Email);
                rol = "Paciente";
            }

            if (usuario == null)
                return NotFound("Usuario inexistente");

            string passwordHash = (string)usuario.GetType().GetProperty("Password")?.GetValue(usuario);

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, passwordHash))
                return Unauthorized("Contraseña incorrecta");

            var usuarioViewModel = new RegisteredViewModel
            {
                Nombre = usuario.GetType().GetProperty("Nombre")?.GetValue(usuario)?.ToString(),
                Apellido = usuario.GetType().GetProperty("Apellido")?.GetValue(usuario)?.ToString(),
                Dni = usuario.GetType().GetProperty("Dni")?.GetValue(usuario)?.ToString(),
                Email = usuario.GetType().GetProperty("Email")?.GetValue(usuario)?.ToString(),
                FechaNacimiento = usuario.GetType().GetProperty("FechaNacimiento")?.GetValue(usuario) as DateTime?,
                Telefono = usuario.GetType().GetProperty("Telefono")?.GetValue(usuario)?.ToString(),
                Direccion = usuario.GetType().GetProperty("Direccion")?.GetValue(usuario)?.ToString(),
                IdRol = rol switch
                {
                    "Administrador" => 1,
                    "Médico" => 2,
                    "Paciente" => 3,
                    _ => 0
                }
            };

            var token = FuncionesToken.GenerarToken(usuarioViewModel, rol, _confi);
            return Ok(token);
        }


    }
}
