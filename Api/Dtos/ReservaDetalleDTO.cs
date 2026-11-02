namespace Api.Dtos
{
    public class ReservaDetalleDTO
    {
        public int idReserva { get; set; }
        public string ServicioNombre { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string BarberoNombre { get; set; }
        public string Estado { get; set; }
    }
}