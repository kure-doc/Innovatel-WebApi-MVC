namespace ServicioWebApiUser.DTOS
{
    public class dtoSeguimientoOrdenCompra
    {
        public int IdPedido { get; set; }
        public string? IdCliente { get; set; }
        public string? NombreCliente { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public DateTime? FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int Cantidad { get; set; }
        public double? TotalDescuento { get; set; }
        public string? ConfirmarPedido { get; set; }
    }
}
