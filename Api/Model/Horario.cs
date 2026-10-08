using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Model
{
    [Table("horario")]
    public class Horario
    {

        [Key]
        public int idHorario { get; set; }
        public int Empleado_idEmpleado { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int Activo { get; set; }
        public Empleado Empleado { get; set; }
    }
}
