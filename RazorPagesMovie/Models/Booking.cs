using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity; // Needed for IdentityUser reference

namespace RazorPagesMovie.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Display(Name = "Time Slot")]
        public int MovieTimeSlotId { get; set; }
        public MovieTimeSlot MovieTimeSlot { get; set; } = default!;

        // Add a field to track which Identity user made the booking
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Display(Name = "User Name (Display Only)")]
        // This field is likely redundant if using UserId, but kept if you need a display name
        public string? UserName { get; set; }

        public string MovieTitle { get; set; }
        public DateTime ShowTime { get; set; }

        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal TotalPrice { get; set; }

        
    }
}
