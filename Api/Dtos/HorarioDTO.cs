using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class HorarioDTO
    {

        [Key]
        public int Empleado_idEmpleado { get; set; }
        public string DiaSemana { get; set; } // Día de la semana
        public TimeSpan HoraInicio { get; set; } // Hora de inicio
        public TimeSpan HoraFin { get; set; } // Hora de fin
        public int Activo { get; set; }
    }
}
