using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }="";

        [Required]
        [EmailAddress] 
        public string Email { get; set; }="";

        [Required]
        public string Rol { get; set; }="Conserje";


    }
}
