using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Postsistem.Models
{
   public class Local
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    public ICollection<Producto> Productos { get; set; }
    public ICollection<Venta> Ventas { get; set; }
    public ICollection<Caja> Cajas { get; set; }
}

} 