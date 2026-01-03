using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postsistem.Data;
using Postsistem.Models;

namespace Postsistem.Controllers
{
    public class SalidasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalidasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salidas/Crear/5
        [HttpGet]
        public async Task<IActionResult> Crear(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Salidas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int productoId, int cantidad, string? nota)
        {
            var producto = await _context.Productos.FindAsync(productoId);

            if (producto == null)
                return NotFound();

            if (cantidad <= 0)
                ModelState.AddModelError("cantidad", "La cantidad debe ser mayor a 0.");

            if (cantidad > producto.Cantidad)
                ModelState.AddModelError("cantidad", $"Stock disponible: {producto.Cantidad}");

            if (!ModelState.IsValid)
                return View(producto);

            // ✅ Descontar stock
            producto.Cantidad -= cantidad;

            // ✅ Registrar salida
            var salida = new Salida
            {
                ProductoId = productoId,
                Cantidad = cantidad,
                Nota = nota
            };

            _context.Salidas.Add(salida);
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Salida registrada correctamente.";
            return RedirectToAction("Index", "Inventario");
        }
    }
}
