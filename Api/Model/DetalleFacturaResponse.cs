namespace Api.Model
{
    public class DetalleFacturaResponse
    {

        public int ProductoId { get; set; } // ID del producto
        public string NombreProducto { get; set; } // Nombre del producto
        public int Cantidad { get; set; } // Cantidad comprada
        public decimal Precio { get; set; } // Precio unitario del producto
        public decimal Subtotal => Cantidad * Precio; // Subtotal calculado (Cantidad * Precio)
    }
}

