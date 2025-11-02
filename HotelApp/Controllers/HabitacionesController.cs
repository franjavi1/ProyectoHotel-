using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models;
using LogicaDeNegocio.Data;
using HotelWeb.Helpers;

namespace HotelWeb.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly AppDbContext _context;

        public HabitacionesController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Método para verificar si el usuario es Administrador
        private bool EsAdministrador()
        {
            // Tomamos el email del claim
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return false;

            var empleado = _context.Empleados.FirstOrDefault(e => e.Email == email);
            return empleado != null && empleado.Rol == "Administrador";
        }


        // GET: Habitaciones
        public async Task<IActionResult> Index()
        {
            await ActualizarDisponibilidadHabitaciones();
            var habitaciones = await _context.Habitaciones.ToListAsync();
            return View(habitaciones);
        }

        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(m => m.Id == id);

            if (habitacion == null) return NotFound();

            return View(habitacion);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            if (!EsAdministrador()) return Forbid();
            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Tipo,Piso,Precio,Disponible")] Habitacion habitacion)
        {
            if (!EsAdministrador()) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!EsAdministrador()) return Forbid();
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();

            return View(habitacion);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Tipo,Piso,Precio,Disponible")] Habitacion habitacion)
        {
            if (!EsAdministrador()) return Forbid();
            if (id != habitacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Habitaciones.Any(e => e.Id == habitacion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!EsAdministrador()) return Forbid();
            if (id == null) return NotFound();

            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacion == null) return NotFound();

            return View(habitacion);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!EsAdministrador()) return Forbid();

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion != null)
            {
                try
                {
                    _context.Habitaciones.Remove(habitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (SqlExceptionHelper.IsForeignKeyViolation(ex))
                {
                    // Mensaje claro para el usuario
                    TempData["Error"] = "No se puede eliminar porque tiene reservas asociadas.";

                }
                catch (Exception)
                {
                    TempData["Error"] = "Ocurrió un error al eliminar el registro.";

                }
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task ActualizarDisponibilidadHabitaciones()
        {
            var habitaciones = await _context.Habitaciones.ToListAsync();
            var reservas = await _context.Reservas.ToListAsync();

            foreach (var habitacion in habitaciones)
            {
                var ocupada = reservas.Any(r =>
                    r.HabitacionId == habitacion.Id &&
                    r.FechaEntrada <= DateTime.Now &&
                    r.FechaSalida >= DateTime.Now);

                habitacion.Disponible = !ocupada;
                _context.Update(habitacion);
            }

            await _context.SaveChangesAsync();
        }



    }
}
