using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Postsistem.Models
{
    public class Venta
    {
        public Venta()
        {
            Productos = new List<ProductoVenta>();
            MetodosPago = new List<MetodoPago>();
        }

        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string? NombreCliente { get; set; }
        public string? CelularCliente { get; set; }
        public string? CedulaCliente { get; set; }

        public List<ProductoVenta> Productos { get; set; }
        public List<MetodoPago> MetodosPago { get; set; }

        // 🔑 CLAVE
        public int LocalId { get; set; }
        public Local Local { get; set; }

        // Descuento GLOBAL en VALOR
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoTotal { get; set; }

        // Caja
        public int? CajaId { get; set; }
        public Caja? Caja { get; set; }

       /* ===== CÁLCULOS ===== */

        [NotMapped]
        public decimal Subtotal => Productos.Sum(p => p.Total);

        [NotMapped]
        public decimal Total => Subtotal - DescuentoTotal;

        [NotMapped]
        public decimal TotalPagado => MetodosPago.Sum(m => m.Valor);

        [NotMapped]
        public decimal SaldoPendiente => Total - TotalPagado;

        /* ===== FORMATEO ===== */

        [NotMapped]
        public string SubtotalFormateado => Subtotal.ToString("N0", CultureInfo.InvariantCulture);

        [NotMapped]
        public string TotalFormateado => Total.ToString("N0", CultureInfo.InvariantCulture);

        [NotMapped]
        public string DescuentoFormateado => DescuentoTotal.ToString("N0", CultureInfo.InvariantCulture);

        [NotMapped]
        public string TotalPagadoFormateado => TotalPagado.ToString("N0", CultureInfo.InvariantCulture);

        [NotMapped]
        public string SaldoPendienteFormateado => SaldoPendiente.ToString("N0", CultureInfo.InvariantCulture);
    }
}