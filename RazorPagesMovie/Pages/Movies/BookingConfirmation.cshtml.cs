// Pages/Movies/BookingConfirmation.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Threading.Tasks;

// Ensure this namespace matches your structure
namespace RazorPagesMovie.Pages.Movies
{
    public class BookingConfirmationModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public BookingConfirmationModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? bookingId)
        {
            if (bookingId == null) return NotFound("Booking ID not provided.");

            // Fetch the booking record, including the related time slot and cinema for display
            Booking = await _context.Booking
                .Include(b => b.MovieTimeSlot)
                    .ThenInclude(ts => ts.Cinema)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (Booking == null) return NotFound("Booking record not found.");

            return Page();
        }
    }
}