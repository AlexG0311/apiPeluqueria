namespace Api.Dtos
{
    public class LoginResponseDto
    {
        public string TipoUsuario { get; set; } // "Cliente" o "Empleado"
        public int idUsuario { get; set; } // ID del usuario genérico
        public int? idCliente { get; set; } // Solo si es cliente
        public int? idEmpleado { get; set; } // Solo si es empleado
    }
}
