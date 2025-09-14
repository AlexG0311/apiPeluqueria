namespace Api.Model
{
    public class DetalleFacturaDTO
    {

        public int ProductoId { get; set; } // ID del producto
        public int Cantidad { get; set; } // Cantidad comprada del producto
        public decimal Precio { get; set; } // Precio unitario del producto
    }
}
