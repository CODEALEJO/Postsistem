using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postsistem.Data;
using Postsistem.Models;
using System.Security.Claims;

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

            // ✅ Obtener el usuario actual
            var usuarioActual = User.Identity?.Name ?? "Usuario no autenticado";
            
            // ✅ Obtener el local del usuario
            int localId = ObtenerLocalDelUsuario();
            
            if (localId == 0)
            {
                TempData["ErrorMessage"] = "El usuario no tiene un local asignado.";
                return View(producto);
            }

            // ✅ Descontar stock
            producto.Cantidad -= cantidad;

            // ✅ Registrar salida
            var salida = new Salida
            {
                ProductoId = productoId,
                Cantidad = cantidad,
                Nota = nota,
                Usuario = usuarioActual, // 🔥 ASIGNAR USUARIO
                LocalId = localId,       // 🔥 ASIGNAR LOCAL
                Tipo = TipoSalida.Ajuste // 🔥 ASIGNAR TIPO (puedes cambiarlo según necesites)
            };

            _context.Salidas.Add(salida);
            _context.Productos.Update(producto);
            
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Salida registrada correctamente.";
                return RedirectToAction("Index", "Inventario");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al registrar la salida: {ex.Message}";
                return View(producto);
            }
        }

        private int ObtenerLocalDelUsuario()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return 0;

            return _context.UsuarioLocales
                .Where(u => u.UserId == userId)
                .Select(u => u.LocalId)
                .FirstOrDefault();
        }
    }
}