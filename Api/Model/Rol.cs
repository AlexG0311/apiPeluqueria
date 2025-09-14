using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{

    [Table("rol")]
    public class Rol
    {

        [Key]
        public int idRol { get; set; }
        public string rolname { get; set; }

        // Relación muchos a muchos con Usuario
        public ICollection<Rolusuario> RolesUsuario { get; set; }
    }
}
