using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models
{
    public class Huesped
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public string Nombre { get; set; } = "";
        
        [Required]
        public string Documento { get; set; } = "";


        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

    
    }
}
