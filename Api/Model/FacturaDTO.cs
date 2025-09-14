namespace Api.Model
{
    public class FacturaDTO
    {

        public DateTime FechaFactura { get; set; } // Fecha en la que se genera la factura
        public decimal Total { get; set; } // Total del monto de la factura
        public List<DetalleFacturaDTO> Detalles { get; set; } // Lista de los detalles de los productos incluidos en la factura
    }
}
