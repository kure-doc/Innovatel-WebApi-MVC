namespace ConsumiendoSWAPI.DTOS
{
    public class dtoListadoPeddosPorCodCliente
    {
        public int IdPedido { get; set; }
        public string? IdCliente { get; set; }
        public int IdProducto { get; set; }
        public DateTime FechaPedido { get; set; }
        public string? MetodoPago { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double TotalDescuento { get; set; }
    }
}
