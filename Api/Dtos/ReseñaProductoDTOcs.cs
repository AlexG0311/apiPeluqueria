using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class ReseñaProductoDTO
    {
        [Key]
        public int idReseñaProducto { get; set; }
        public int Estrellas { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public int Cliente_idCliente { get; set; }
        public int Producto_idProducto { get; set; }
    }
}
