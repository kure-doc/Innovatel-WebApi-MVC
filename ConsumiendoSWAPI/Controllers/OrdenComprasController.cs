using ConsumiendoSWAPI.DTOS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioWebApiUser.DTOS;
using ServicioWebApiUser.Models;
using System.Net.Http;
using System.Text;

namespace ConsumiendoSWAPI.Controllers
{
    public class OrdenComprasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdenComprasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var lgpedidoscompra = new List<OrdenCompra>();
            using (var httpclient = new HttpClient())
            {
                using (var repo = await httpclient.GetAsync("https://localhost:7128/api/OrdenCompras"))
                {
                    string apires = await repo.Content.ReadAsStringAsync();
                    lgpedidoscompra = JsonConvert.DeserializeObject<List<OrdenCompra>>(apires);
                }
            }

            return View(lgpedidoscompra);
        }
        [HttpGet]
        public async Task<IActionResult> AgregarOrdenCompra()
        {
            try
            {
                await CargarProductos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading clients and products: {ex.Message}");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> confirmacionCompra(int id, string mensaje)
        {
            OrdenCompra compra = null;
            using (var httpclient = new HttpClient())
            {
                var response = await httpclient.GetAsync($"https://localhost:7128/api/OrdenCompras/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string apiRes = await response.Content.ReadAsStringAsync();
                    compra = JsonConvert.DeserializeObject<OrdenCompra>(apiRes);
                }
            }

            if (compra == null)
            {
                return NotFound();
            }

            // Verifica el estado de la compra
            if (compra.ConfirmarPedido == "Confirmado" || compra.ConfirmarPedido == "Rechazado")
            {
                TempData["Alerta"] = $"La compra ya ha sido {compra.ConfirmarPedido}.";
                return RedirectToAction("Index");
            }

            ViewBag.Mensaje = mensaje;
            return View(compra);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarCompra(OrdenCompra ordenCompra)
        {
            try
            {
                if (ordenCompra == null)
                {
                    return BadRequest("Invalid data.");
                }

                Console.WriteLine($"IdProducto: {ordenCompra.IdProducto}, Cantidad: {ordenCompra.Cantidad}");

                // Verificar el estado de la compra antes de actualizar
                OrdenCompra compraActual = null;
                using (var httpclient = new HttpClient())
                {
                    var response = await httpclient.GetAsync($"https://localhost:7128/api/OrdenCompras/{ordenCompra.IdOrdenCompra}");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiRes = await response.Content.ReadAsStringAsync();
                        compraActual = JsonConvert.DeserializeObject<OrdenCompra>(apiRes);
                    }
                }

                if (compraActual == null || compraActual.ConfirmarPedido == "Confirmado" || compraActual.ConfirmarPedido == "Rechazado")
                {
                    TempData["Alerta"] = "La compra ya ha sido confirmada o rechazada y no puede ser modificada.";
                    return RedirectToAction("Index");
                }

                var dtoCompra = new dtoCompras
                {
                    IdOrdenCompra = ordenCompra.IdOrdenCompra,
                    ConfirmarPedido = ordenCompra.ConfirmarPedido
                };

                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(dtoCompra), Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();
                using (var response = await httpClient.PostAsync("https://localhost:7128/api/OrdenCompras/ActualizarCompra", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (ordenCompra.ConfirmarPedido == "Confirmado")
                        {
                            var dtoCompraUpdate = new dtoCompra
                            {
                                IdProducto = ordenCompra.IdProducto,
                                Cantidad = ordenCompra.Cantidad
                            };

                            StringContent updateContent = new StringContent(
                                JsonConvert.SerializeObject(dtoCompraUpdate),
                                Encoding.UTF8, "application/json");

                            var updateResponse = await httpClient.PostAsync("https://localhost:7128/api/OrdenCompras/AumentarCantidad", updateContent);
                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                string errorMsg = await updateResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"Error actualizando la cantidad del producto: {errorMsg}");
                            }
                        }
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error en la API: {apiResponse}");
                    }
                }

                TempData["Alerta"] = $"La compra ha sido {ordenCompra.ConfirmarPedido}.";
                return RedirectToAction("Index");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"NullReferenceException: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AgregarOrdenCompra(OrdenCompra ordenCompra)
        {
            // Validar que la cantidad sea mayor a 0
            if (ordenCompra.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");
            }

            if (!ModelState.IsValid)
            {
                await CargarProductos(); // Asegúrate de que esto no cause problemas
                return View(ordenCompra);
            }

            ordenCompra.FechaCompra = DateTime.Now;

            ordenCompra.TotalCompra = ordenCompra.Cantidad * ordenCompra.Precio;

            try
            {
                using (var httpclient = new HttpClient())
                {
                    StringContent content = new StringContent(
                        JsonConvert.SerializeObject(ordenCompra), Encoding.UTF8, "application/json");

                    using (var resp = await httpclient.PostAsync("https://localhost:7128/api/OrdenCompras", content))
                    {
                        string apiResp = await resp.Content.ReadAsStringAsync();
                        if (resp.IsSuccessStatusCode)
                        {
                            try
                            {
                                var ordenCompra1 = JsonConvert.DeserializeObject<OrdenCompra>(apiResp);

                                // Verificar si ordenCompra1 no es null
                                if (ordenCompra1 == null)
                                {
                                    throw new Exception("No se pudo deserializar la orden de compra.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error deserializando la respuesta: {ex.Message}");
                                ModelState.AddModelError(string.Empty, $"Error al procesar la respuesta del servidor: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error en la respuesta de la API: {apiResp}");
                            ModelState.AddModelError(string.Empty, $"Error en la API: {apiResp}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error en AgregarOrdenCompra: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Error al agregar la orden de compra: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }



        private async Task CargarProductos()
        {
            // Cargar Productos
            List<dtoProductoNom> lgproducto = new List<dtoProductoNom>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var respuesta = await httpClient.GetAsync("https://localhost:7128/api/Productoes/ListaDeProductosNom");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resApi = await respuesta.Content.ReadAsStringAsync();
                        lgproducto = JsonConvert.DeserializeObject<List<dtoProductoNom>>(resApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }
            ViewBag.Productos = lgproducto != null && lgproducto.Any()
                ? lgproducto
                : new List<dtoProductoNom>();

            // Opcional: JSON para JavaScript
            ViewBag.ProductosJson = JsonConvert.SerializeObject(lgproducto);
        }
    }
}
