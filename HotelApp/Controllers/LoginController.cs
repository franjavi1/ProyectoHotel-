using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HotelWeb.Controllers
{
    public class LoginController : Controller
    {
        // Muestra la vista Login.cshtml
        public IActionResult Login()
        {
            return View(); // Busca Views/Login/Login.cshtml
        }

        // Inicia la autenticación con Google
        public async Task<IActionResult> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Procesa la respuesta de Google y redirige a Home
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                // Obtenemos datos del usuario
                var nombre = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                // Guardamos el nombre en TempData para mostrarlo en Home
                TempData["Usuario"] = nombre;

                // Opcional: aquí podés guardar o actualizar el usuario en la base de datos
            }

            // Redirige a la página principal
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login"); // vuelve a la pantalla de login
        }

    }
}
