

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Threading.Tasks;

public class CancelModel : PageModel
{
    private readonly RazorPagesMovieContext _context;

    [BindProperty]
    public Booking Booking { get; set; } = default!;

    public CancelModel(RazorPagesMovieContext context)
    {
        _context = context;
    }

    // OnGet: Display confirmation details
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        Booking = await _context.Booking
            .Include(b => b.MovieTimeSlot)
            .Include(b => b.MovieTimeSlot.Movie)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Booking == null) return NotFound();

        return Page();
    }

    // OnPost: Execute cancellation and update seats
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var bookingToCancel = await _context.Booking
            .Include(b => b.MovieTimeSlot) // Crucial to load the time slot for seat update
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bookingToCancel == null) return NotFound();

        // 1. Return the tickets/seats to the available count for the showtime
        if (bookingToCancel.MovieTimeSlot != null)
        {
            bookingToCancel.MovieTimeSlot.AvailableSeats += bookingToCancel.NumberOfTickets;
            _context.MovieTimeSlot.Update(bookingToCancel.MovieTimeSlot);
        }

        // 2. Remove the booking record
        _context.Booking.Remove(bookingToCancel);

        // 3. Save all changes
        await _context.SaveChangesAsync();

        // Redirect back to the main bookings list
        return RedirectToPage("./MyBookings");
    }
}