using System.ComponentModel.DataAnnotations;

namespace Api.ClinicaMedica.DTO.Basic
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
              
    }
}
