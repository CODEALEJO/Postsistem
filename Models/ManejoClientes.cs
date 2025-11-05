using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Postsistem.Models
{
    public class ManejoClientes
    {
        public ManejoClientes()
        {
            Abonos = new List<AbonoCliente>();
            DevolucionesGarantias = new List<DevolucionGarantia>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [MaxLength(100)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula o documento es obligatoria")]
        [MaxLength(20)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Phone]
        [MaxLength(15)]
        public string? Celular { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoPendiente { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoAFavor { get; set; } = 0;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relaciones
        public List<AbonoCliente> Abonos { get; set; }
        public List<DevolucionGarantia> DevolucionesGarantias { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public decimal TotalAbonado => Abonos?.Sum(a => a.Monto) ?? 0;

        [NotMapped]
        public decimal TotalDevoluciones => DevolucionesGarantias?.Sum(d => d.Monto) ?? 0;

        [NotMapped]
        public bool PazYSalvo => (SaldoPendiente - TotalAbonado - TotalDevoluciones) <= 0;

        [NotMapped]
        public string Estado => PazYSalvo ? "Paz y salvo" : "Debe dinero";

        [NotMapped]
        public string SaldoPendienteFormateado => SaldoPendiente.ToString("N0", CultureInfo.InvariantCulture);

        [NotMapped]
        public string SaldoAFavorFormateado => SaldoAFavor.ToString("N0", CultureInfo.InvariantCulture);
    }
}
