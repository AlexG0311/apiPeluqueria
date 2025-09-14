using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{
    [Table("clienteproducto")]
    public class clienteproducto
    {
        [Key]
        public int cliente_idCliente { get; set; }
        public Cliente Cliente { get; set; }
        public int producto_idProducto { get; set; }

        public Producto Producto { get; set; }
    }
}
