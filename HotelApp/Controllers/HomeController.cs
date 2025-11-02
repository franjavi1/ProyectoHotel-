using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models;
using LogicaDeNegocio.Data;
using System.Security.Claims;

namespace HotelApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Obtener correo real del usuario autenticado
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            // Lista de administradores
            var allowedEmails = new List<string>
            {
                "franciscojavierstevenin@gmail.com",
                "diego_rg@hotmail.com",
                "diego.r.gallo@gmail.com"
            };

            // Verificar si es administrador
            if (!allowedEmails.Contains(userEmail))
            {
                return RedirectToAction("AccessDenied");
            }

            // Periodo: últimos 6 meses (incluye mes actual)
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var months = Enumerable.Range(0, 6)
                                   .Select(i => startMonth.AddMonths(i))
                                   .ToList();

            var labels = months.Select(m => m.ToString("MMM yyyy")).ToArray();

            var reservasCounts = new List<int>();
            var ingresos = new List<decimal>();

            foreach (var month in months)
            {
                var from = month;
                var to = month.AddMonths(1);

                var reservasMes = await _context.Reservas
                    .Where(r => r.FechaEntrada >= from && r.FechaEntrada < to)
                    .ToListAsync();

                reservasCounts.Add(reservasMes.Count);
                ingresos.Add(reservasMes.Sum(r => r.PrecioTotal));
            }

            // Totales y estado de pago
            var totalReservas = await _context.Reservas.CountAsync();
            var pagadas = await _context.Reservas.CountAsync(r => r.Pagado);
            var noPagadas = totalReservas - pagadas;

            // Habitaciones totales y ocupadas ahora
            var totalHabitaciones = await _context.Habitaciones.CountAsync();
            var ocupadasAhora = await _context.Reservas
                .CountAsync(r => r.FechaEntrada <= now && r.FechaSalida >= now);

            // Pasar datos a la vista como JSON
            ViewData["LabelsJson"] = JsonSerializer.Serialize(labels);
            ViewData["ReservasCountsJson"] = JsonSerializer.Serialize(reservasCounts);
            ViewData["IngresosJson"] = JsonSerializer.Serialize(ingresos);
            ViewData["PagosJson"] = JsonSerializer.Serialize(new[] { pagadas, noPagadas });
            ViewData["SummaryJson"] = JsonSerializer.Serialize(new
            {
                TotalReservas = totalReservas,
                Pagadas = pagadas,
                NoPagadas = noPagadas,
                TotalHabitaciones = totalHabitaciones,
                OcupadasAhora = ocupadasAhora
            });

            ViewData["Title"] = "Estadísticas";
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Acceso Denegado";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
