using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Admin
{
    [Authorize(Roles = "Admin")] // Admin only
    public class ViewBookingsModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public ViewBookingsModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; }

        public async Task OnGetAsync()
        {
            // Load bookings with related data
            Bookings = await _context.Booking
                .Include(b => b.MovieTimeSlot)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.MovieTimeSlot)
                    .ThenInclude(s => s.Cinema)
                .ToListAsync();
        }
    }
}
