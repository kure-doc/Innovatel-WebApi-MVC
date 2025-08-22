
namespace ServicioWebApiUser.DTOS
{
    using Newtonsoft.Json;

    public class dtoPedidosporMes
    {
        [JsonProperty("idPedido")]
        public int IdPedido { get; set; }

        [JsonProperty("idCliente")]
        public string? IdCliente { get; set; }

        [JsonProperty("idProducto")]
        public int IdProducto { get; set; }

        [JsonProperty("nombreMes")]
        public string? NombreMes { get; set; }

        [JsonProperty("metodoPago")]
        public string? MetodoPago { get; set; }

        [JsonProperty("cantidad")]
        public int Cantidad { get; set; }

        [JsonProperty("precio")]
        public double Precio { get; set; }

        [JsonProperty("totalDescuento")]
        public double TotalDescuento { get; set; }

        [JsonProperty("confirmarPedido")]
        public string? ConfirmarPedido { get; set; }
    }
}
