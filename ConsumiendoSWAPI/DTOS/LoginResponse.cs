namespace ServicioWebApiUser.DTOS
{
    public class LoginResponse
    {
        public bool IsAuthenticated { get; set; }
        public string? IdCliente { get; set; }
    }
}
