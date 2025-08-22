using Newtonsoft.Json;

namespace ServicioWebApiUser.DTOS
{
    public class dtoMesesdeVenta
    {
        [JsonProperty("nromes")]
        public int Nromes { get; set; }

        [JsonProperty("nombremes")]
        public string? NombreMes { get; set; }
    }
}
