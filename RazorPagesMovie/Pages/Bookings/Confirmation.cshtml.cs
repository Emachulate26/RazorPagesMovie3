using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Bookings
{
    public class ConfirmationModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public ConfirmationModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; } = default!;

        public async Task OnGetAsync(int id)
        {
            Booking = await _context.Booking
                .Include(b => b.MovieTimeSlot)
                .ThenInclude(mts => mts.Cinema)
                .Include(b => b.MovieTimeSlot)
                .ThenInclude(mts => mts.Movie)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
