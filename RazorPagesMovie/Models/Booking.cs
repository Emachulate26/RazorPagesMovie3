using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int MovieTimeSlotId { get; set; }
        public MovieTimeSlot MovieTimeSlot { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
