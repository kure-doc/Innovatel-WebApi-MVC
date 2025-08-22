using System.ComponentModel.DataAnnotations;

namespace ServicioWebApiUser.Models
{
    public class OrdenPedido
    {
        [Display(Name = "Id Pedido")]
        public int IdPedido { get; set; }
        [Required(ErrorMessage = "El Id no han sido ingresados")]
        [Display(Name = "Id Cliente")]
        public string? IdCliente { get; set; }
        [Required(ErrorMessage = "El Id no han sido ingresados")]
        [Display(Name = "Id Producto")]
        public int IdProducto { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaPedido { get; set; }
        [Required(ErrorMessage = "El Metodo de Pago no han sido ingresados")]
        [Display(Name = "Método de Pago")]
        public string? MetodoPago { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double TotalDescuento { get; set; }
        [Display(Name = "Estado del pedido")]
        public string? ConfirmarPedido { get; set; }
    }
}
