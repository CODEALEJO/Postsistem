using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Postsistem.Models
{

      public enum TipoSalida
    {
        Garantia = 1,
        Regalo = 2,
        Ajuste = 3
    }
    
    public class Salida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [StringLength(255)]
        public string? Nota { get; set; }

        public DateTime FechaSalida { get; set; } = DateTime.Now;

        public int LocalId { get; set; }
        public Local Local { get; set; }

        public string Usuario { get; set; }

        public TipoSalida Tipo { get; set; } // Garantia, Regalo, Ajuste

    }
}
