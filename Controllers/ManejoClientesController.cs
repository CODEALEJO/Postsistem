using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postsistem.Models;
using Postsistem.Data;
using System.Threading.Tasks;

namespace Postsistem.Controllers
{
    public class ManejoClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManejoClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================================
        // 📜 LISTAR TODOS LOS CLIENTES
        // ======================================================
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.ManejoClientes
                .Include(c => c.Abonos)
                .Include(c => c.DevolucionesGarantias)
                .ToListAsync();

            return View(clientes);
        }

        // ======================================================
        // ➕ CREAR CLIENTE (GET)
        // ======================================================
        public IActionResult Create()
        {
            return View();
        }

        // ➕ CREAR CLIENTE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManejoClientes cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.FechaRegistro = DateTime.UtcNow;
                _context.ManejoClientes.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // ======================================================
        // ✏️ EDITAR CLIENTE (GET)
        // ======================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.ManejoClientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ✏️ EDITAR CLIENTE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ManejoClientes cliente)
        {
            if (id != cliente.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // ======================================================
        // 🗑️ ELIMINAR CLIENTE (GET)
        // ======================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.ManejoClientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // 🗑️ ELIMINAR CLIENTE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.ManejoClientes.FindAsync(id);
            if (cliente != null)
            {
                _context.ManejoClientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // 💰 REGISTRAR ABONO (GET)
        // ======================================================
        public async Task<IActionResult> RegistrarAbono(int id)
        {
            var cliente = await _context.ManejoClientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            ViewBag.ClienteNombre = cliente.NombreCliente;
            ViewBag.ClienteId = cliente.Id;
            return View();
        }

        // 💰 REGISTRAR ABONO (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAbono(AbonoCliente abono)
        {
            if (ModelState.IsValid)
            {
                abono.FechaAbono = DateTime.UtcNow;
                _context.AbonoClientes.Add(abono);
                await _context.SaveChangesAsync();

                // actualizar saldo pendiente
                var cliente = await _context.ManejoClientes.FindAsync(abono.ManejoClientesId);
                if (cliente != null)
                {
                    cliente.SaldoPendiente -= abono.Monto;
                    if (cliente.SaldoPendiente < 0)
                    {
                        cliente.SaldoAFavor = Math.Abs(cliente.SaldoPendiente);
                        cliente.SaldoPendiente = 0;
                    }

                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Details), new { id = abono.ManejoClientesId });
            }

            return View(abono);
        }

        // ======================================================
        // 🔁 REGISTRAR DEVOLUCIÓN / GARANTÍA (GET)
        // ======================================================
        public async Task<IActionResult> RegistrarDevolucion(int id)
        {
            var cliente = await _context.ManejoClientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            ViewBag.ClienteNombre = cliente.NombreCliente;
            ViewBag.ClienteId = cliente.Id;
            return View();
        }

        // 🔁 REGISTRAR DEVOLUCIÓN / GARANTÍA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarDevolucion(DevolucionGarantia devolucion)
        {
            if (ModelState.IsValid)
            {
                devolucion.FechaDevolucion = DateTime.UtcNow;
                _context.DevolucionGarantias.Add(devolucion);
                await _context.SaveChangesAsync();

                // actualizar saldo o saldo a favor
                var cliente = await _context.ManejoClientes.FindAsync(devolucion.ManejoClientesId);
                if (cliente != null)
                {
                    cliente.SaldoAFavor += devolucion.Monto;
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Details), new { id = devolucion.ManejoClientesId });
            }

            return View(devolucion);
        }

        // ======================================================
        // 🔍 DETALLES / ESTADO FINANCIERO
        // ======================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.ManejoClientes
                .Include(c => c.Abonos)
                .Include(c => c.DevolucionesGarantias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ======================================================
        // 🔐 MÉTODO PRIVADO
        // ======================================================
        private bool ClienteExists(int id)
        {
            return _context.ManejoClientes.Any(e => e.Id == id);
        }
    }
}
