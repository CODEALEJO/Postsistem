using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postsistem.Data;
using Postsistem.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Postsistem.Controllers
{
    public class InventarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string nombre = null)
        {
            int localId = ObtenerLocalDelUsuario();

            if (localId == 0)
                return Unauthorized("El usuario no tiene un local asignado.");

            IQueryable<Producto> productos = _context.Productos
                .Include(p => p.Salidas)
                .Where(p => p.LocalId == localId)
                .OrderBy(p => p.Nombre);

            if (!string.IsNullOrEmpty(nombre))
                productos = productos.Where(p => p.Nombre.Contains(nombre));

            return View(await productos.ToListAsync());
        }

        [HttpGet]
        public IActionResult BuscarProducto(string nombre)
        {
            int localId = ObtenerLocalDelUsuario();

            var producto = _context.Productos
                .Where(p => p.LocalId == localId &&
                            p.Nombre.ToLower().Contains(nombre.ToLower()))
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    nombre = p.Nombre,
                    cantidad = p.Cantidad,
                    precioVenta = p.PrecioVenta
                })
                .FirstOrDefault();

            return Json(producto);
        }

        [HttpGet]
        public IActionResult CreateMultiple()
        {
            return View(new List<Producto> { new Producto() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(List<Producto> productos)
        {
            try
            {
                Console.WriteLine($"DEBUG: Cantidad de productos recibidos: {productos?.Count}");

                int localId = ObtenerLocalDelUsuario();

                if (localId == 0)
                {
                    TempData["ErrorMessage"] = "El usuario no tiene un local asignado.";
                    return Unauthorized();
                }

                Console.WriteLine($"DEBUG: LocalId obtenido: {localId}");

                // 🔥 CORRECCIÓN IMPORTANTE: El model binding ahora usa List<Producto> sin prefijo
                // Ya no necesitamos remover productos[0].LocalId, sino [0].LocalId
                for (int i = 0; i < productos.Count; i++)
                {
                    var key = $"[{i}].LocalId";
                    ModelState.Remove(key);
                }

                // 🔥 VALIDACIÓN: Filtrar productos vacíos (solo nombre en blanco)
                var productosValidos = new List<Producto>();
                for (int i = 0; i < productos.Count; i++)
                {
                    // IMPORTANTE: productos puede tener valores nulos para elementos vacíos dinámicos
                    if (productos[i] != null &&
                        !string.IsNullOrWhiteSpace(productos[i].Nombre) &&
                        productos[i].Cantidad > 0)
                    {
                        productosValidos.Add(productos[i]);
                    }
                }

                // Si no hay productos válidos
                if (productosValidos.Count == 0)
                {
                    TempData["ErrorMessage"] = "Debe ingresar al menos un producto válido.";
                    return View(new List<Producto> { new Producto() });
                }

                // 🔥 VALIDACIÓN DE MODELO: Solo validar productos no vacíos
                var isValid = true;
                var validationErrors = new List<string>();

                for (int i = 0; i < productosValidos.Count; i++)
                {
                    var validationContext = new ValidationContext(productosValidos[i], null, null);
                    var validationResults = new List<ValidationResult>();

                    if (!Validator.TryValidateObject(productosValidos[i], validationContext, validationResults, true))
                    {
                        isValid = false;
                        foreach (var result in validationResults)
                        {
                            validationErrors.Add($"Producto {i + 1}: {result.ErrorMessage}");
                        }
                    }
                }

                if (!isValid)
                {
                    TempData["ErrorMessage"] = $"Errores de validación: {string.Join(" | ", validationErrors)}";
                    // Devolvemos la vista con los productos válidos para que no se pierdan los datos
                    return View(productosValidos);
                }

                // 🔥 GUARDAR PRODUCTOS VÁLIDOS
                foreach (var producto in productosValidos)
                {
                    producto.LocalId = localId;
                    _context.Productos.Add(producto);
                    Console.WriteLine($"DEBUG: Agregando producto: {producto.Nombre}, Cantidad: {producto.Cantidad}");
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{productosValidos.Count} producto(s) agregado(s) correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"ERROR DB: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"INNER EXCEPTION: {dbEx.InnerException.Message}");
                }

                TempData["ErrorMessage"] = $"Error de base de datos: {dbEx.Message}";
                return View(productos ?? new List<Producto> { new Producto() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL: {ex.Message}");
                Console.WriteLine($"STACK TRACE: {ex.StackTrace}");

                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(productos ?? new List<Producto> { new Producto() });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            int localId = ObtenerLocalDelUsuario();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.LocalId == localId);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            int localId = ObtenerLocalDelUsuario();

            if (id != producto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                producto.LocalId = localId; // seguridad

                _context.Update(producto);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Producto actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            int localId = ObtenerLocalDelUsuario();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.LocalId == localId);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int localId = ObtenerLocalDelUsuario();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.LocalId == localId);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Producto eliminado correctamente";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            int localId = ObtenerLocalDelUsuario();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.LocalId == localId);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        private int ObtenerLocalDelUsuario()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return 0;

            return _context.UsuarioLocales
                .Where(u => u.UserId == userId)
                .Select(u => u.LocalId)
                .FirstOrDefault();
        }
    }
}
