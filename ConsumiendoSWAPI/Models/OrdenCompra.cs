using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioWebApiUser.Models
{
    public class OrdenCompra
    {
        [Key]
        public int IdOrdenCompra { get; set; }
        [Required(ErrorMessage = "El Id no han sido ingresados")]
        [Display(Name = "ID Producto")]
        public int IdProducto { get; set; }
        public string? NroCompra { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? NombreProveedor { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCompra {  get; set; } = DateTime.Now;
        public double Precio { get; set; }
        public int Cantidad { get; set; }
        public double? TotalCompra { get; set; }
        [Display(Name = "Estado del la compra")]
        public string? ConfirmarPedido { get; set; }
    }
}
