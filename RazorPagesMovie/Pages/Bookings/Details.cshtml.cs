

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Threading.Tasks;

public class DetailsModel : PageModel
{
    private readonly RazorPagesMovieContext _context;

    // Property to hold the single booking we are viewing
    public Booking Booking { get; set; } = default!;

    public DetailsModel(RazorPagesMovieContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Fetch the specific booking by ID, loading all related data
        Booking = await _context.Booking
            .Include(b => b.MovieTimeSlot)      // Load the specific time slot
            .Include(b => b.MovieTimeSlot.Movie) // Load the movie details
            .Include(b => b.MovieTimeSlot.Cinema) // Include the Cinema details
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Booking == null)
        {
            return NotFound();
        }
        return Page();
    }
}