using System.ComponentModel.DataAnnotations;

namespace ServicioWebApiUser.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }
        public string? IdCliente { get; set; }
        public string? NombreProveedor { get; set; }
        public string? DireccionProveedor { get; set; }
        public string? TelefonoProveedor { get; set; }
    }
}
