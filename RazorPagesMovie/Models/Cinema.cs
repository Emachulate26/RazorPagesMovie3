using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        // Navigation
        public List<MovieTimeSlot> TimeSlots { get; set; }
    }
}
