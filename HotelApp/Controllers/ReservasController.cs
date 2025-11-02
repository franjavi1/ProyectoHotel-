using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models;
using LogicaDeNegocio.Data;

namespace HotelWeb.Controllers
{
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reservas.Include(r => r.Empleado).Include(r => r.Habitacion).Include(r => r.Huesped);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Empleado)
                .Include(r => r.Habitacion)
                .Include(r => r.Huesped)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email");
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero");
            ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HuespedId,HabitacionId,EmpleadoId,FechaEntrada,FechaSalida,PrecioTotal,Pagado")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // ✅ Verificar superposición de reservas para la misma habitación
                bool haySuperposicion = await _context.Reservas
                    .AnyAsync(r =>
                        r.HabitacionId == reserva.HabitacionId &&
                        (
                            // Si las fechas se solapan (al menos un día en común)
                            (reserva.FechaEntrada <= r.FechaSalida && reserva.FechaSalida >= r.FechaEntrada)
                        )
                    );

                if (haySuperposicion)
                {
                    ModelState.AddModelError("", "Ya existe una reserva para esta habitación en el rango de fechas seleccionado.");

                    // Volvemos a armar los combos para la vista
                    ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email", reserva.EmpleadoId);
                    ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
                    ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido", reserva.HuespedId);
                    return View(reserva);
                }

                // ✅ Si no hay superposición, grabamos la reserva
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email", reserva.EmpleadoId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido", reserva.HuespedId);
            return View(reserva);
        }


        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email", reserva.EmpleadoId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido", reserva.HuespedId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HuespedId,HabitacionId,EmpleadoId,FechaEntrada,FechaSalida,PrecioTotal,Pagado")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // ✅ Verificar que no haya otra reserva que se solape con las fechas editadas
                bool haySuperposicion = await _context.Reservas
                    .AnyAsync(r =>
                        r.HabitacionId == reserva.HabitacionId &&
                        r.Id != reserva.Id && // 👈 excluimos la misma reserva
                        (
                            reserva.FechaEntrada <= r.FechaSalida &&
                            reserva.FechaSalida >= r.FechaEntrada
                        )
                    );

                if (haySuperposicion)
                {
                    ModelState.AddModelError("", "Ya existe una reserva para esta habitación en el rango de fechas seleccionado.");

                    ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email", reserva.EmpleadoId);
                    ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
                    ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido", reserva.HuespedId);
                    return View(reserva);
                }

                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmpleadoId"] = new SelectList(_context.Empleados, "Id", "Email", reserva.EmpleadoId);
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HuespedId"] = new SelectList(_context.Clientes, "Id", "Apellido", reserva.HuespedId);
            return View(reserva);
        }


        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Empleado)
                .Include(r => r.Habitacion)
                .Include(r => r.Huesped)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}