using System.ComponentModel.DataAnnotations;

namespace ServicioWebApiUser.DTOS
{
    public class dtoLoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string? Contrasenia { get; set; }
    }
}
