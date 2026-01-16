using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Postsistem.Models
{
    public class ProductoVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Relación opcional con inventario
        public int? ProductoId { get; set; }
        public Producto? Producto { get; set; }

        // Nombre congelado en la venta (histórico)
        [Required]
        [MaxLength(150)]
        public string NombreProducto { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

    

        // Descuento en VALOR (no porcentaje)
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoValor { get; set; }

        // Precio final unitario
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioFinal { get; set; }

        // Relación con venta
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        // ====== CALCULADOS ======

        [NotMapped]
        public decimal Total => Math.Round(Cantidad * PrecioFinal, 2);

        [NotMapped]
        public string TotalFormateado => Total.ToString("N0", CultureInfo.InvariantCulture);
    }
}
