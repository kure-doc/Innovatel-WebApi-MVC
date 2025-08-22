using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioWebApiUser.DTOS;
using ServicioWebApiUser.Models;
using System.Text;


namespace ConsumiendoSWAPI.Controllers
{
    public class ProveedorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SeguimientoOrdenCompra()
        {
            var idCliente = User.FindFirst("IdCliente")?.Value;

            if (string.IsNullOrEmpty(idCliente))
            {
                return RedirectToAction("Login", "Perfil");
            }

            List<dtoSeguimientoOrdenCompra> seguimientoOrdenCompraList;
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    $"https://localhost:7128/api/Proveedors/SeguimientoOrdenCompra/{idCliente}"))
                {
                    string resapi = await resp.Content.ReadAsStringAsync();
                    seguimientoOrdenCompraList = JsonConvert.DeserializeObject<List<dtoSeguimientoOrdenCompra>>(resapi);
                }
            }

            // Manejar el mensaje de alerta
            ViewBag.AlertMessage = HttpContext.Request.Query["alert"];
            return View(seguimientoOrdenCompraList);
        }

        public async Task<IActionResult> DetallesPedido(int idPedido)
        {
            var idCliente = User.FindFirst("IdCliente")?.Value;

            if (string.IsNullOrEmpty(idCliente))
            {
                return RedirectToAction("Login", "Perfil");
            }

            dtoSeguimientoOrdenCompra pedidoDetalle;
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    $"https://localhost:7128/api/Proveedors/DetallesPedido/{idPedido}"))
                {
                    string resapi = await resp.Content.ReadAsStringAsync();
                    pedidoDetalle = JsonConvert.DeserializeObject<dtoSeguimientoOrdenCompra>(resapi);
                }
            }

            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            return View(pedidoDetalle);
        }

        public async Task<IActionResult> ConfirmarCompra(int idPedido)
        {
            dtoSeguimientoOrdenCompra pedidoDetalle;

            // Obtener los detalles del pedido
            using (var httpclient = new HttpClient())
            {
                using (var resp = await httpclient.GetAsync(
                    $"https://localhost:7128/api/Proveedors/DetallesPedido/{idPedido}"))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        // Manejar el error al obtener los detalles del pedido
                        return RedirectToAction("SeguimientoOrdenCompra", new { alert = "Error al obtener los detalles del pedido." });
                    }

                    string resapi = await resp.Content.ReadAsStringAsync();
                    pedidoDetalle = JsonConvert.DeserializeObject<dtoSeguimientoOrdenCompra>(resapi);
                }
            }

            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            // Confirmar la compra
            using (var httpclient = new HttpClient())
            {
                var response = await httpclient.PostAsync(
                    $"https://localhost:7128/api/Proveedors/ConfirmarCompra/{idPedido}", null);

                if (response.IsSuccessStatusCode)
                {
                    // Llamar a DisminuirCantidad
                    var dtoVenta = new dtoVenta
                    {
                        IdProducto = pedidoDetalle.IdProducto,
                        Cantidad = pedidoDetalle.Cantidad
                    };

                    var cantidadContent = new StringContent(
                        JsonConvert.SerializeObject(dtoVenta),
                        Encoding.UTF8,
                        "application/json");

                    // Verificar la ruta para DisminuirCantidad
                    var disminucionResponse = await httpclient.PostAsync(
                        "https://localhost:7128/api/OrdenPedidoes/DisminuirCantidad",
                        cantidadContent);

                    if (disminucionResponse.IsSuccessStatusCode)
                    {
                        return RedirectToAction("DetallesPedido", new { idPedido = idPedido });
                    }
                    else
                    {
                        // Leer el mensaje de error de la respuesta
                        string errorMsg = await disminucionResponse.Content.ReadAsStringAsync();
                        return RedirectToAction("SeguimientoOrdenCompra", new { alert = $"Error al disminuir la cantidad del producto: {errorMsg}" });
                    }
                }
                else
                {
                    // Leer el mensaje de error de la respuesta de ConfirmarCompra
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("SeguimientoOrdenCompra", new { alert = $"Error al confirmar la compra: {errorMsg}" });
                }
            }
        }



        public async Task<IActionResult> RechazarCompra(int idPedido)
        {
            using (var httpclient = new HttpClient())
            {
                var response = await httpclient.PostAsync(
                    $"https://localhost:7128/api/Proveedors/RechazarCompra/{idPedido}", null);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DetallesPedido", new { idPedido = idPedido });
                }
                else
                {
                    // Manejar el error
                    return RedirectToAction("SeguimientoOrdenCompra", new { alert = "Error al rechazar la compra." });
                }
            }
        }

    }
}
