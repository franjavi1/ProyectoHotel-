using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


[Authorize(Roles = "Administrador")] // ✅ Solo administradores pueden entrar
public class EstadisticasController : Controller
{
    public IActionResult Index()
    {
        // Datos de ejemplo (vos los reemplazás con datos reales de tu base)
        var labels = new[] { "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre" };
        var reservasCounts = new[] { 12, 15, 9, 18, 20, 16 };
        var ingresos = new[] { 10000, 15000, 12000, 18000, 20000, 16000 };
        var pagos = new[] { 70, 20 }; // [Pagadas, No Pagadas]

        var summary = new
        {
            TotalReservas = 90,
            Pagadas = 70,
            NoPagadas = 20,
            OcupadasAhora = 45,
            TotalHabitaciones = 60
        };

        // Convertir a JSON para que el View lo use
        ViewData["LabelsJson"] = JsonSerializer.Serialize(labels);
        ViewData["ReservasCountsJson"] = JsonSerializer.Serialize(reservasCounts);
        ViewData["IngresosJson"] = JsonSerializer.Serialize(ingresos);
        ViewData["PagosJson"] = JsonSerializer.Serialize(pagos);
        ViewData["SummaryJson"] = JsonSerializer.Serialize(summary);

        return View(); // Va a buscar Views/Estadisticas/Index.cshtml
    }
}
