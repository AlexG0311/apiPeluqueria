namespace Api.Model
{
    public class FacturaResponse
    {
        public int IdFactura { get; set; } // ID único de la factura generada
        public DateTime FechaFactura { get; set; } // Fecha en la que se generó la factura
        public decimal Total { get; set; } // Total de la factura
        public List<DetalleFacturaResponse> Detalles { get; set; } // Lista de los detalles de la factura
    }
}
