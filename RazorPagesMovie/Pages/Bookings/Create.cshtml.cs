using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(RazorPagesMovieContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public SelectList MovieTimeSlots { get; set; } = default!;
        public Movie? Movie { get; set; }

        // Accept movieId as a query parameter
        public async Task<IActionResult> OnGetAsync(int? movieId)
        {
            if (movieId == null)
                return NotFound();

            Movie = await _context.Movie.FindAsync(movieId);
            if (Movie == null)
                return NotFound();

            // Only show timeslots for the selected movie
            var slots = await _context.MovieTimeSlot
                .Include(s => s.Cinema)
                .Where(s => s.MovieId == movieId && s.AvailableSeats > 0)
                .ToListAsync();

            MovieTimeSlots = new SelectList(slots, "Id", "DisplayName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload the MovieTimeSlots if the form has errors
                var selectedSlot = await _context.MovieTimeSlot
                    .Include(s => s.Movie)
                    .Include(s => s.Cinema)
                    .FirstOrDefaultAsync(s => s.Id == Booking.MovieTimeSlotId);

                if (selectedSlot != null)
                {
                    MovieTimeSlots = new SelectList(
                        _context.MovieTimeSlot
                            .Where(s => s.MovieId == selectedSlot.MovieId && s.AvailableSeats > 0)
                            .ToList(),
                        "Id", "DisplayName");
                }

                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            Booking.UserId = user?.Id;
            Booking.UserName = user?.UserName;

            var slot = await _context.MovieTimeSlot
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == Booking.MovieTimeSlotId);

            if (slot != null)
            {
                Booking.MovieTitle = slot.Movie.Title;
                Booking.ShowTime = slot.ShowTime;
                Booking.Price = slot.Price * Booking.NumberOfTickets;
                Booking.BookingDate = DateTime.Now;

                // Reduce available seats
                slot.AvailableSeats -= Booking.NumberOfTickets;
            }

            _context.Booking.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Bookings/Details", new { id = Booking.Id });
        }
    }
}
