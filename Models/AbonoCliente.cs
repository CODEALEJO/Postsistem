using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Postsistem.Models
{
    public class AbonoCliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManejoClientesId { get; set; }

        [ForeignKey(nameof(ManejoClientesId))]
        public ManejoClientes Cliente { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El abono debe ser un valor positivo")]
        public decimal Monto { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaAbono { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Observacion { get; set; }
    }
}
