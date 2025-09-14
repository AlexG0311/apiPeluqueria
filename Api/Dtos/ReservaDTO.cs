namespace Api.Dtos
{
    public class ReservaDTO
    {
        public DateTime Fecha { get; set; } // Fecha seleccionada para la reserva
        public TimeSpan Hora { get; set; } // Hora seleccionada para la reserva
        public int Cliente_idCliente { get; set; } // ID del cliente que realiza la reserva
        public int Servicio_idServicio { get; set; } // ID del servicio seleccionado
        public int Empleado_idEmpleado { get; set; } // ID del empleado asignado
        public int Estado_idEstado { get; set; } // Estado de la reserva (por ejemplo: 1 = Pendiente)
    }
}
