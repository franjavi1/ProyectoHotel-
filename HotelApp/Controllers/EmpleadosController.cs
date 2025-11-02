using HotelApp.Models;
using LogicaDeNegocio.Data;
using Microsoft.Data.SqlClient;
using HotelWeb.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace HotelWeb.Controllers
{
    [Authorize(Roles = "Administrador")] // ✅ Solo administradores pueden acceder
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _context;

        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            return View(await _context.Empleados.ToListAsync());
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == id);
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Rol")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,Rol")] Empleado empleado)
        {
            if (id != empleado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Empleados.Any(e => e.Id == empleado.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == id);
            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                try
                {
                    _context.Empleados.Remove(empleado);
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

        /* La muevo al Helper asi la pueden usar todos
        // Detecta violación de FK en SQL Server (error 547)
        private static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            // busca en InnerException y en la base
            if (ex.InnerException is SqlException sql && sql.Number == 547) return true;
            if (ex.GetBaseException() is SqlException b && b.Number == 547) return true;
            return false;
        }*/

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
