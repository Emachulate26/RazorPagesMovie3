using System;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class MovieTimeSlot
    {
        public int Id { get; set; }

        // FK to Movie
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public DateTime ShowTime { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Time {  get; set; }

        //Fk ScreenNumber is defined
        public int ScreenNumber { get; set; }

        // FK to Cinema
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Range(0, 300)]
        public int AvailableSeats { get; set; }
    }
}
