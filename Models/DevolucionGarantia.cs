using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Postsistem.Models
{
    public class DevolucionGarantia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManejoClientesId { get; set; }

        [ForeignKey(nameof(ManejoClientesId))]
        public ManejoClientes Cliente { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser un valor positivo")]
        public decimal Monto { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaDevolucion { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Motivo { get; set; }
    }
}
