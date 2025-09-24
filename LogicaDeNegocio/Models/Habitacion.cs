using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelApp.Models
{
    public class Habitacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Numero { get; set; }="";

        [Required]
        public string Tipo { get; set; } = "";

        [Required]
        public int Piso { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public bool Disponible { get; set; } = true;


    }
}
