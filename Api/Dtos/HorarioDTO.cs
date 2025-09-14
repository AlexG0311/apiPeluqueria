using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class HorarioDTO
    {

        [Key]
        public DateTime DiaMesAño { get; set; } // Fecha seleccionada
        public TimeSpan HoraInicio { get; set; } // Hora de inicio
        public TimeSpan HoraFin { get; set; } // Hora de fin
        public int Empleado_idEmpleado { get; set; } // ID del empleado seleccionado
    }
}
