using ConsumiendoSWAPI.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ServicioWebApiUser.Models;
using ServicioWebApiUser.DTOS;
using System.Text;

namespace ConsumiendoSWAPI.Controllers
{
    public class ProductoController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var lgproductos = new List<Producto>();
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    "https://localhost:7128/api/Productoes"))
                {
                    string apiresp = await resp.Content.ReadAsStringAsync();
                    lgproductos = JsonConvert.DeserializeObject<List<Producto>>(apiresp);
                }
            }
            return View(lgproductos);
        }
        public async Task<IActionResult> BuscarProducto(string? codigo)
        {
            Console.WriteLine($"Valor recibido de codigo: {codigo}");

            List<dtoProducto> lgproducto = new List<dtoProducto>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var respuesta = await httpClient.GetAsync("https://localhost:7128/api/Productoes/ListadeProductos");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resApi = await respuesta.Content.ReadAsStringAsync();
                        lgproducto = JsonConvert.DeserializeObject<List<dtoProducto>>(resApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }

            if (lgproducto != null && lgproducto.Any())
            {
                ViewBag.producto = new SelectList(lgproducto, "codigoPro", "codigoPro", codigo);
            }
            else
            {
                ViewBag.producto = new SelectList(Enumerable.Empty<dtoProducto>(), "codigoPro", "codigoPro");
            }

            List<dtoPro> lgconsulta = new List<dtoPro>();

            try
            {
                if (!string.IsNullOrEmpty(codigo))
                {
                    Console.WriteLine($"Consultando API con el código: {codigo}");
                    using (var clienteHttp = new HttpClient())
                    {
                        var respuesta = await clienteHttp.GetAsync($"https://localhost:7128/api/Productoes/PedidoPorBusquedadeCodigo/{codigo}");
                        if (respuesta.IsSuccessStatusCode)
                        {
                            string resApi = await respuesta.Content.ReadAsStringAsync();
                            var testJson = "[{\"id\":1,\"nombre\":\"Producto1\",\"codigo\":\"ABC123\",\"precio\":29.99}]";
                            lgconsulta = JsonConvert.DeserializeObject<List<dtoPro>>(resApi);
                        }
                        else
                        {
                            Console.WriteLine($"La API no devolvió un estado exitoso: {respuesta.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product details: {ex.Message}");
            }

            lgconsulta = lgconsulta ?? new List<dtoPro>();

            return View(lgconsulta);
        }


        [HttpGet]
        public IActionResult AgregarProducto()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgregarProducto(Producto producto)
        {
            Producto producto1 = new Producto();
            using (var httpclient = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(producto), Encoding.UTF8, "application/json");
                using (var resp = await httpclient.PostAsync("https://localhost:7128/api/Productoes", content))
                {
                    string apiResp = await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        try
                        {
                            producto1 = JsonConvert.DeserializeObject<Producto>(apiResp);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deserializando la respuesta: {ex.Message}");
                            ModelState.AddModelError(string.Empty, "Error al procesar la respuesta del servidor.");
                            return View(producto);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error en la respuesta de la API: {apiResp}");
                        ModelState.AddModelError(string.Empty, $"Error en la API: {apiResp}");
                        return View(producto);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProducto(int id)
        {
            Producto user = new Producto();
            using ( var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    "https://localhost:7128/api/Productoes/" + id))
                {
                    string resapi = await resp.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Producto>(resapi);
                }
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProducto(int id, Producto producto)
        {
            Producto producto1 = new();
            using ( var httpclient = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(producto), Encoding.UTF8,
                    "application/json");
                using (var resp = await httpclient.PutAsync(
                    "https://localhost:7128/api/Productoes/" + id, content))
                {
                    string apiresp = await resp.Content.ReadAsStringAsync();
                    producto1 = JsonConvert.DeserializeObject<Producto>(apiresp);
                    ViewBag.Producto = producto1;
                }
            }
            return View(producto1);
        }
        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.DeleteAsync(
                    "https://localhost:7128/api/Productoes/" + id))
                {

                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
