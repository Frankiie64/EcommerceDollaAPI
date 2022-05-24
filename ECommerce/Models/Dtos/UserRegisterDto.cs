using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Models.Dtos
{
    public class UserRegisterDto
    {

        [Required(ErrorMessage = "Por favor introduzca su nombre.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Por favor introduzca sus apellidos.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Por favor introduzca su nombre de usuario.")]

        public string Username { get; set; }
        [Required(ErrorMessage = "Por favor introduzca su numero de telefono.")]

        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Por favor introduzca su correo electronico.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Por favor introduzca una contraseña.")]
        [StringLength(12, ErrorMessage = "La {0} debe ser mayor a {2} caracteres y menor a {1}.", MinimumLength = 8)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}
