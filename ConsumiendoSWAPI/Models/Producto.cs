using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioWebApiUser.Models
{
    public class Producto
    {
        [Display(Name = "Id Producto")]
        public int IdProducto { get; set; }
        [Required(ErrorMessage = "El Nombre del Producto no han sido ingresados")]
        [Display(Name = "Nombre del Producto")]
        public string? NombreProducto { get; set; }
        [Required(ErrorMessage = "El codigo del producto no han sido ingresados")]
        [Display(Name = "Código del Producto")]
        public string? CodigoProducto { get; set; }
        [Display(Name = "Precio Producto")]
        public double? PrecioProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
