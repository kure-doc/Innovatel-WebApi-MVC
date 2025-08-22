using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioWebApiUser.DTOS;
using System.Security.Claims;
using System.Text;

namespace ConsumiendoSWAPI.Controllers
{
    public class PerfilController : Controller
    {
        private readonly string apiBaseUrl = "https://localhost:7128/api/Perfiles";

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(dtoLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(
                        JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{apiBaseUrl}/Login", content);
                    response.EnsureSuccessStatusCode();

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);

                    if (loginResponse != null && loginResponse.IsAuthenticated)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Nombre),
                    new Claim("IdCliente", loginResponse.IdCliente ?? string.Empty), 
                    new Claim(ClaimTypes.Role, model.Nombre.Equals("administrador", StringComparison.OrdinalIgnoreCase) ? "Administrador" : "Usuario")

                };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        // Redireccionar según el nombre de usuario
                        if (model.Nombre.Equals("administrador", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Proveedor");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al procesar la solicitud.");
            }

            return View(model);
        }



        // Método para cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
