using ConsumiendoSWAPI.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ServicioWebApiUser.DTOS;
using ServicioWebApiUser.Models;
using System.Text;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Net.Http;

namespace ConsumiendoSWAPI.Controllers
{
    public class OrdenPedidoesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdenPedidoesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var lgpedidoventa = new List<OrdenPedido>();
            using (var httpclient = new HttpClient())
            {
                using (var reso = await httpclient.GetAsync("https://localhost:7128/api/OrdenPedidoes"))
                {
                    string apires = await reso.Content.ReadAsStringAsync();
                    lgpedidoventa = JsonConvert.DeserializeObject<List<OrdenPedido>>(apires);
                }
            }

            // Filtrar solo los estados deseados
            var pedidosFiltrados = lgpedidoventa
                .Where(p => p.ConfirmarPedido == "Confirmado" ||
                             p.ConfirmarPedido == "Rechazado" ||
                             p.ConfirmarPedido == "Pendiente Confirmar")
                .ToList();

            return View(lgpedidoventa);
        }

        public async Task<IActionResult> ListaPedidoCliente(string? codcli)
        {
            List<dtoListadoPeddosPorCodCliente> lgpedido = new List<dtoListadoPeddosPorCodCliente>();
            using (var clientehttp = new HttpClient())
            {
                using (var respuesta = await clientehttp.GetAsync("https://localhost:7128/api/OrdenPedidoes/ListadodePedidosporCodCliente/" + codcli))
                {
                    string resApi = await respuesta.Content.ReadAsStringAsync();
                    lgpedido = JsonConvert.DeserializeObject<List<dtoListadoPeddosPorCodCliente>>(resApi);
                }
            }
            lgpedido = lgpedido ?? new List<dtoListadoPeddosPorCodCliente>();

            ViewBag.codigo = codcli;

            return View(lgpedido);
        }

        public async Task<IActionResult> ExportarExcel()
        {
            var lgpedidoventa = new List<OrdenPedido>();
            using (var httpclient = new HttpClient())
            {
                using (var reso = await httpclient.GetAsync("https://localhost:7128/api/OrdenPedidoes"))
                {
                    string apires = await reso.Content.ReadAsStringAsync();
                    lgpedidoventa = JsonConvert.DeserializeObject<List<OrdenPedido>>(apires);
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pedidos");

                // Encabezados de la tabla
                worksheet.Cells["A1"].Value = "IdPedido";
                worksheet.Cells["B1"].Value = "IdCliente";
                worksheet.Cells["C1"].Value = "IdProducto";
                worksheet.Cells["D1"].Value = "FechaPedido";
                worksheet.Cells["E1"].Value = "MetodoPago";
                worksheet.Cells["F1"].Value = "Cantidad";
                worksheet.Cells["G1"].Value = "Precio";
                worksheet.Cells["H1"].Value = "TotalDescuento";
                worksheet.Cells["I1"].Value = "ConfirmarPedido";

                // Aplicar estilo a los encabezados
                using (var range = worksheet.Cells["A1:I1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Borde inferior
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin; // Borde superior
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin; // Borde izquierdo
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                // Datos de la tabla
                var row = 2;
                foreach (var item in lgpedidoventa)
                {
                    worksheet.Cells[$"A{row}"].Value = item.IdPedido;
                    worksheet.Cells[$"B{row}"].Value = item.IdCliente;
                    worksheet.Cells[$"C{row}"].Value = item.IdProducto;
                    worksheet.Cells[$"D{row}"].Value = item.FechaPedido.ToString("yyyy-MM-dd");
                    worksheet.Cells[$"E{row}"].Value = item.MetodoPago;
                    worksheet.Cells[$"F{row}"].Value = item.Cantidad;
                    worksheet.Cells[$"G{row}"].Value = item.Precio;
                    worksheet.Cells[$"H{row}"].Value = item.TotalDescuento;
                    worksheet.Cells[$"I{row}"].Value = item.ConfirmarPedido;

                    // Aplicar borde a las celdas de datos
                    using (var range = worksheet.Cells[$"A{row}:I{row}"])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid; // Definir el tipo de patrón
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Borde inferior
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin; // Borde superior
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin; // Borde izquierdo
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    }
                    row++;
                }

                // Autoajustar el ancho de las columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Pedidos.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public async Task<IActionResult> ExportarExcelMesVenta(int nromes)
        {
            List<dtoMesesdeVenta> lgmesesventa = new List<dtoMesesdeVenta>();
            string nombreMes = "Desconocido"; // Nombre por defecto en caso de error

            // Obtener el nombre del mes
            using (var clientehttp = new HttpClient())
            {
                using (var respuesta = await clientehttp.GetAsync(
                    "https://localhost:7128/api/OrdenPedidoes/MesesdeVenta"))
                {
                    string resApi = await respuesta.Content.ReadAsStringAsync();
                    lgmesesventa = JsonConvert.DeserializeObject<List<dtoMesesdeVenta>>(resApi);
                    var mes = lgmesesventa.FirstOrDefault(m => m.Nromes == nromes);
                    if (mes != null)
                    {
                        nombreMes = mes.NombreMes;
                    }
                }
            }

            var reportesVentasMensuales = new List<dtoPedidosporMes>();
            using (var httpclient = new HttpClient())
            {
                var url = $"https://localhost:7128/api/OrdenPedidoes/PedidoporMes/{nromes}";
                using (var reso = await httpclient.GetAsync(url))
                {
                    string apires = await reso.Content.ReadAsStringAsync();
                    reportesVentasMensuales = JsonConvert.DeserializeObject<List<dtoPedidosporMes>>(apires);
                }
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("VentasMensuales" + nromes);

                // Encabezados de la tabla
                worksheet.Cells["A1"].Value = "Id Pedido";
                worksheet.Cells["B1"].Value = "Id Cliente";
                worksheet.Cells["C1"].Value = "Id Producto";
                worksheet.Cells["D1"].Value = "Nombre del Mes";
                worksheet.Cells["E1"].Value = "Metodo de Pago";
                worksheet.Cells["F1"].Value = "Cantidad";
                worksheet.Cells["G1"].Value = "Precio";
                worksheet.Cells["H1"].Value = "Total Descuento";
                worksheet.Cells["I1"].Value = "Confirmación del Pedido";

                // Aplicar estilo a los encabezados
                using (var range = worksheet.Cells["A1:I1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                // Datos de la tabla
                var row = 2;
                foreach (var item in reportesVentasMensuales)
                {
                    worksheet.Cells[$"A{row}"].Value = item.IdPedido;
                    worksheet.Cells[$"B{row}"].Value = item.IdCliente;
                    worksheet.Cells[$"C{row}"].Value = item.IdProducto;
                    worksheet.Cells[$"D{row}"].Value = item.NombreMes;
                    worksheet.Cells[$"E{row}"].Value = item.MetodoPago;
                    worksheet.Cells[$"F{row}"].Value = item.Cantidad;
                    worksheet.Cells[$"G{row}"].Value = item.Precio;
                    worksheet.Cells[$"H{row}"].Value = item.TotalDescuento;
                    worksheet.Cells[$"I{row}"].Value = item.ConfirmarPedido;

                    // Aplicar borde a las celdas de datos
                    using (var range = worksheet.Cells[$"A{row}:I{row}"])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    row++;
                }

                // Autoajustar el ancho de las columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Usar el nombre del mes para el archivo
                var fileName = $"Ventas_del_Mes_de_{nombreMes}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        public async Task<IActionResult> ConsultaMesdeVenta(int? nromes)
        {
            List<dtoMesesdeVenta> lgmesesventa = new List<dtoMesesdeVenta>();
            using (var clientehttp = new HttpClient())
            {
                using (var respuesta = await clientehttp.GetAsync(
                    "https://localhost:7128/api/OrdenPedidoes/MesesdeVenta"))
                {
                    string resApi = await respuesta.Content.ReadAsStringAsync();
                    lgmesesventa = JsonConvert.DeserializeObject<List<dtoMesesdeVenta>>(resApi);
                }
            }

            // Asegúrate de usar los nombres correctos de las propiedades del DTO
            ViewBag.mesesdeventa = new SelectList(lgmesesventa, "Nromes", "NombreMes", nromes);

            List<dtoPedidosporMes> lgpedido = new List<dtoPedidosporMes>();
            if (nromes.HasValue) // Asegúrate de que nromes no sea null antes de usarlo
            {
                using (var clienteHttp = new HttpClient())
                {
                    using (var respuesta = await clienteHttp.GetAsync(
                        $"https://localhost:7128/api/OrdenPedidoes/PedidoporMes/{nromes.Value}"))
                    {
                        string resApi = await respuesta.Content.ReadAsStringAsync();
                        lgpedido = JsonConvert.DeserializeObject<List<dtoPedidosporMes>>(resApi);
                    }
                }
            }

            lgpedido = lgpedido ?? new List<dtoPedidosporMes>();

            return View(lgpedido);
        }

        [HttpGet]
        public async Task<IActionResult> AgregarPedidoVenta()
        {
            try
            {
                await CargarClientesYProductos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading clients and products: {ex.Message}");
                // Manejar el error apropiadamente, como redirigir a una página de error
                // return View("Error");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPedido([FromBody] dtoPedido pedidoDto)
        {
            if (pedidoDto == null)
            {
                return BadRequest("Invalid data.");
            }



            StringContent content = new StringContent(
                JsonConvert.SerializeObject(pedidoDto), Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient();
            using (var response = await httpClient.PostAsync("https://localhost:7128/api/OrdenPedidoes/ActualizarPedido", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(apiResponse); // Devuelve la respuesta de la API
                }
                else
                {
                    return BadRequest($"Error en la API: {apiResponse}");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPedidoVenta(OrdenPedido ordenpedido)
        {
            // Validar que la cantidad sea mayor a 0
            if (ordenpedido.Cantidad <= 0)
            {
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");
            }

            if (!ModelState.IsValid)
            {
                await CargarClientesYProductos(); // Volver a cargar los datos en caso de error
                return View(ordenpedido);
            }

            // Asignar la fecha actual a FechaPedido si no se ha proporcionado
            ordenpedido.FechaPedido = DateTime.Now;

            // Obtener información del producto para validar stock
            Producto producto = null;
            using (var productoHttpClient = new HttpClient())
            {
                var productoResponse = await productoHttpClient.GetAsync($"https://localhost:7128/api/Productoes/{ordenpedido.IdProducto}");
                if (productoResponse.IsSuccessStatusCode)
                {
                    string productoResApi = await productoResponse.Content.ReadAsStringAsync();
                    producto = JsonConvert.DeserializeObject<Producto>(productoResApi);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se pudo obtener información del producto.");
                    await CargarClientesYProductos(); // Volver a cargar los datos en caso de error
                    return View(ordenpedido);
                }
            }

            // Validar que la cantidad no supere el stock
            if (ordenpedido.Cantidad > producto.Cantidad)
            {
                string productoInfo = $"Producto: {producto.NombreProducto}, Cantidad disponible: {producto.Cantidad}";
                ModelState.AddModelError(string.Empty, $"El stock supera la cantidad en el almacén. {productoInfo}");

                // Si hay un error de stock, no registrar la venta y volver a cargar los datos
                await CargarClientesYProductos(); // Volver a cargar los datos en caso de error
                return View(ordenpedido);
            }

            using (var httpclient = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(ordenpedido), Encoding.UTF8, "application/json");

                using (var resp = await httpclient.PostAsync("https://localhost:7128/api/OrdenPedidoes", content))
                {
                    string apiResp = await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        // Aquí puedes realizar otras operaciones si es necesario
                    }
                    else
                    {
                        Console.WriteLine($"Error en la respuesta de la API: {apiResp}");
                        ModelState.AddModelError(string.Empty, $"Error en la API: {apiResp}");
                    }
                }
            }

            await CargarClientesYProductos(); // Volver a cargar los datos al finalizar
            return RedirectToAction(nameof(Index));
        }


        private async Task CargarClientesYProductos()
        {
            // Cargar Clientes
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
            }
            ViewBag.cliente = lgcliente != null && lgcliente.Any()
                ? new SelectList(lgcliente, "IdCliente", "NombreCli")
                : new SelectList(Enumerable.Empty<dtoCliente>(), "IdCliente", "NombreCli");

            // Cargar Productos
            List<dtoProductoID> lgproducto = new List<dtoProductoID>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var respuesta = await httpClient.GetAsync("https://localhost:7128/api/Productoes/ListaDeProductosID");
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resApi = await respuesta.Content.ReadAsStringAsync();
                        lgproducto = JsonConvert.DeserializeObject<List<dtoProductoID>>(resApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
            }
            ViewBag.Productos = lgproducto != null && lgproducto.Any()
                ? lgproducto
                : new List<dtoProductoID>();

            // Opcional: JSON para JavaScript
            ViewBag.ProductosJson = JsonConvert.SerializeObject(lgproducto);
        }

    }
}
