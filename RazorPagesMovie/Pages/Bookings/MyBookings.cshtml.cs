// Pages/Bookings/MyBookings.cshtml.cs (UPDATED CONTEXT NAME)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
// Using your Data and Models namespaces:
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MyBookingsModel : PageModel
{
    // Use your context name here
    private readonly RazorPagesMovieContext _context;
    private readonly UserManager<IdentityUser> _userManager; // Assuming IdentityUser or your custom user type

    public IList<Booking> Bookings { get; set; } = new List<Booking>();

    // Inject your context
    public MyBookingsModel(RazorPagesMovieContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        if (userId != null)
        {
            // Now using MovieTimeSlot and Movie properties
            Bookings = await _context.Booking
                .Include(b => b.MovieTimeSlot)       // Load the specific time slot
                .Include(b => b.MovieTimeSlot.Movie) // Then load the movie details
                .Where(b => b.UserId == userId)      // Filter by current user
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }
    }
}