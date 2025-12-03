using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Bookings
{
    public class BookModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public BookModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string UserName { get; set; }

        public MovieTimeSlot TimeSlot { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            TimeSlot = await _context.MovieTimeSlot
                .Include(m => m.Movie)
                .Include(c => c.Cinema)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (TimeSlot == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var slot = await _context.MovieTimeSlot
                .Include(m => m.Movie)
                .Include(c => c.Cinema)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (slot == null)
                return NotFound();

            if (slot.AvailableSeats <= 0)
            {
                ModelState.AddModelError("", "Not Available");
                TimeSlot = slot;
                return Page();
            }

            // Reduce seat
            slot.AvailableSeats--;

            // Save booking
            var booking = new Booking
            {
                MovieTimeSlotId = id,
                UserName = UserName
            };

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("Success");
        }
    }
}
