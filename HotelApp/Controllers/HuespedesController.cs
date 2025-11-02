using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelApp.Models;
using LogicaDeNegocio.Data;
using HotelWeb.Helpers;

namespace HotelWeb.Controllers
{
    public class HuespedesController : Controller
    {
        private readonly AppDbContext _context;

        public HuespedesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Huespeds
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Huespeds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (huesped == null)
            {
                return NotFound();
            }

            return View(huesped);
        }

        // GET: Huespeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Huespeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Apellido,Nombre,Documento,Email")] Huesped huesped)
        {
            if (ModelState.IsValid)
            {
                _context.Add(huesped);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(huesped);
        }

        // GET: Huespeds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Clientes.FindAsync(id);
            if (huesped == null)
            {
                return NotFound();
            }
            return View(huesped);
        }

        // POST: Huespeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Apellido,Nombre,Documento,Email")] Huesped huesped)
        {
            if (id != huesped.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(huesped);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HuespedExists(huesped.Id))
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
            return View(huesped);
        }

        // GET: Huespeds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huesped = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (huesped == null)
            {
                return NotFound();
            }

            return View(huesped);
        }

        // POST: Huespeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var huesped = await _context.Clientes.FindAsync(id);
            if (huesped != null)
            {
                try
                {
                    _context.Clientes.Remove(huesped);
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

        private bool HuespedExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
