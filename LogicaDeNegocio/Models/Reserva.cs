using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HuespedId { get; set; }
        public Huesped? Huesped { get; set; }

        [Required]
        public int HabitacionId { get; set; }
        public Habitacion? Habitacion { get; set; }

        [Required]
        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        [Required]
        public DateTime FechaEntrada { get; set; }

        [Required]
        public DateTime FechaSalida { get; set; }

        [Required]
        public decimal PrecioTotal { get; set; }

        [Required]
        public bool Pagado { get; set; } = false;

    }
}
