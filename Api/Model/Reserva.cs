using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Model
{
    [Table("reserva")]
    public class Reserva
    {
        [Key]
        public int idReserva { get; set; }

        public DateTime Fecha { get; set; } // Fecha de la reserva
        public TimeSpan Hora { get; set; } // Hora de la reserva

        [ForeignKey("Empleado")] // Configuración de la clave foránea
        public int Empleado_idEmpleado { get; set; }
        public Empleado Empleado { get; set; } // Relación con Empleado

        [ForeignKey("Servicio")] // Relación con Servicio
        public int Servicio_idServicio { get; set; }
        public Servicio Servicio { get; set; } // Relación con Servicio

        [ForeignKey("Estado")] // Relación con Estado
        public int Estado_idEstado { get; set; }
        public Estado Estado { get; set; } // Relación con Estado


        [Required]
        public int Cliente_idCliente { get; set; } // Llave foránea hacia Cliente
        [ForeignKey("Cliente_idCliente")]
        public Cliente Cliente { get; set; } // Propiedad de navegación hacia Cliente

    }
}
