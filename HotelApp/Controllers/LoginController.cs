using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using HotelApp.Models;
using LogicaDeNegocio.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelWeb.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }


        // Todas las acciones ya no necesitan "if (!EsAdministrador()) return Forbid();"
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }

        public IActionResult Login()
        {
            return View(); // Busca Views/Login/Login.cshtml
        }

        public async Task<IActionResult> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var nombre = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

                TempData["Usuario"] = nombre;

                // Verificar si ya existe en la base
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Email == email);

                if (empleado == null)
                {
                    // Lista de emails que serán administradores
                    var administradores = new List<string>
                    {
                        "franciscojavierstevenin@gmail.com",
                        "compañero1@gmail.com", 
                        "luis271277@gmail.com",
                        "diego.r.gallo@gmail.com"
                    };

                    empleado = new Empleado
                    {
                        Nombre = nombre,
                        Email = email,
                        Rol = administradores.Contains(email) ? "Administrador" : "Conserje"
                    };

                    _context.Empleados.Add(empleado);
                    await _context.SaveChangesAsync();
                }

                // Crear claims incluyendo el rol
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, nombre),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, empleado.Rol) // ✅ Rol agregado
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View(); // Crea Views/Login/AccessDenied.cshtml
        }
    }
}
