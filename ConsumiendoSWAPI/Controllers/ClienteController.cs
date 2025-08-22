using ConsumiendoSWAPI.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ServicioWebApiUser.DTOS;
using ServicioWebApiUser.Models;
using System.Text;

namespace ConsumiendoSWAPI.Controllers
{
    public class ClienteController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var lgclientes = new List<Cliente>();
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    "https://localhost:7128/api/Clientes"))
                {
                    string apiresp = await resp.Content.ReadAsStringAsync();
                    lgclientes = JsonConvert.DeserializeObject<List<Cliente>>(apiresp);
                }
            }
            return View(lgclientes);
        }
        public async Task<IActionResult> BuscarCliente(string? id)
        {
            List<dtoCliente> lgcliente = new List<dtoCliente>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var respuesta = await httpClient.GetAsync("https://localhost:7128/api/Clientes/ListadeClientes");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resApi = await respuesta.Content.ReadAsStringAsync();
                        lgcliente = JsonConvert.DeserializeObject<List<dtoCliente>>(resApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching clients: {ex.Message}");
                lgcliente = new List<dtoCliente>(); // Asegurarse de que no sea null
            }

            if (lgcliente.Any())
            {
                ViewBag.cliente = new SelectList(lgcliente, "IdCliente", "IdCliente", id);
            }
            else
            {
                ViewBag.cliente = new SelectList(Enumerable.Empty<dtoCliente>(), "IdCliente", "IdCliente");
            }

            List<dtoCli> lgconsulta = new List<dtoCli>();

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    using (var clienteHttp = new HttpClient())
                    {
                        var respuesta = await clienteHttp.GetAsync($"https://localhost:7128/api/Clientes/PedidoPorBusquedaDeCliente/{id}");
                        if (respuesta.IsSuccessStatusCode)
                        {
                            string resApi = await respuesta.Content.ReadAsStringAsync();
                            lgconsulta = JsonConvert.DeserializeObject<List<dtoCli>>(resApi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching client details: {ex.Message}");
                lgconsulta = new List<dtoCli>(); // Asegurarse de que no sea null
            }

            return View(lgconsulta);
        }


        [HttpGet]
        public IActionResult AgregarCliente()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgregarCliente(Cliente cliente)
        {
            //-----


            Cliente cliente1 = new Cliente();
            using (var httpCliente = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");

                using (var resp = await httpCliente.PostAsync("https://localhost:7128/api/Clientes", content))
                {
                    string apiResp = await resp.Content.ReadAsStringAsync();

                    // Verificar si la respuesta es JSON válida
                    if (resp.IsSuccessStatusCode)
                    {
                        try
                        {
                            cliente1 = JsonConvert.DeserializeObject<Cliente>(apiResp);
                        }
                        catch (JsonException ex)
                        {
                            // Log del error de deserialización
                            Console.WriteLine($"Error deserializando la respuesta: {ex.Message}");
                            // Manejo de error o retorno de una vista de error
                            ModelState.AddModelError(string.Empty, "Error al procesar la respuesta del servidor.");
                            return View(cliente);
                        }
                    }
                    else
                    {
                        // Log de la respuesta en caso de error
                        Console.WriteLine($"Error en la respuesta de la API: {apiResp}");
                        // Manejo del error de respuesta del servidor
                        ModelState.AddModelError(string.Empty, $"Error en la API: {apiResp}");
                        return View(cliente);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> UpdateCliente(string? id)
        {
            Cliente user = new();
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    "https://localhost:7128/api/Clientes/" + id))
                {
                    string resapi = await resp.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<Cliente>(resapi);
                }
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCliente(string? id, Cliente cliente)
        {
            Cliente cliente1 = new();
            using (var httpclient = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(cliente), Encoding.UTF8,
                    "application/json");
                using (var resp = await httpclient.PutAsync(
                "https://localhost:7128/api/Clientes/" + id, content))
                {
                    string apiresp = await resp.Content.ReadAsStringAsync();
                    cliente1 = JsonConvert.DeserializeObject<Cliente>(apiresp);
                    ViewBag.cliente = cliente1;
                }
            }
            return View(cliente1);
        }
        [HttpPost]
        public async Task<IActionResult> EliminarCliente(string? id)
        {
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.DeleteAsync(
                    "https://localhost:7128/api/Clientes/" + id))
                {

                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
