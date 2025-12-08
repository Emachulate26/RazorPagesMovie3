using RazorPagesMovie.Models;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public int SeatsBooked { get; set; }
    }
}
 