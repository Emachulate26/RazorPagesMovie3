using System;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required]
        [Display(Name = "Show Time")]
        public DateTime Time { get; set; }

        [Required]
        public string Location { get; set; }  // Cinema / Room

        [Display(Name = "Total Seats")]
        public int TotalSeats { get; set; } = 50;

        [Display(Name = "Booked Seats")]
        public int BookedSeats { get; set; } = 0;
    }
}
