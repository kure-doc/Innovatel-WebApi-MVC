using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ServicioWebApiUser.Models
{
    public class Cliente
    {
        [Display(Name = "ID Clientes")]
        public string? IdCliente { get; set; }
        [Required(ErrorMessage = "Los Nombres no han sido ingresados")]
        [Display(Name = "Nombres Clientes")]
        public string? NombreCliente { get; set; }
        [Required(ErrorMessage = "La Dirreccion no ha sido ingresado")]
        [Display(Name = "Dirección")]
        public string? DirreccionCliente { get; set; }
        [Required(ErrorMessage = "No ha ingresado su Telefono")]
        [Display(Name = "Teléfono")]
        [StringLength(9, ErrorMessage = "Longitud de Telefono(9 Digitos)", MinimumLength = 9)]
        public string? TelefonoCliente { get; set; }
        [Required(ErrorMessage = "Ingrese su correo electronico")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de correo incorrecto")]
        [Display(Name = "Correo Electrónico")]
        public string? Email {  get; set; }
    }
}
