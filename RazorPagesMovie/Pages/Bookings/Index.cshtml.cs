using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // REQUIRED for UserManager
using Microsoft.AspNetCore.Authorization; // Good practice for booking pages
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Bookings
{
    // Ensure only logged-in users can view this page
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        // 1. ADDED: Field to hold the user manager
        private readonly UserManager<IdentityUser> _userManager;

        // 2. UPDATED: Constructor now accepts UserManager
        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize the user manager
        }

        public IList<Booking> Booking { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // 3. GET USER ID: Retrieve the unique ID of the currently logged-in user
            var userId = _userManager.GetUserId(User);

            if (_context.Booking != null)
            {
                Booking = await _context.Booking
                    // 4. INCLUDE: Load the related MovieTimeSlot entity
                    .Include(b => b.MovieTimeSlot)
                        // 5. THENINCLUDE: Load the Movie related to that TimeSlot
                        .ThenInclude(mts => mts.Movie)
                    // 6. FILTER: CRITICAL FIX - Only retrieve bookings where the UserId matches the logged-in user's ID
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();
            }
        }
    }
}