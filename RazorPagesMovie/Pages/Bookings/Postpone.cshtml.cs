

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PostponeModel : PageModel
{
    private readonly RazorPagesMovieContext _context;

    [BindProperty]
    public Booking Booking { get; set; } = default!;

    // Property to hold the available showtimes for the user to select
    public SelectList AvailableShowTimes { get; set; } = default!;

    // This property will capture the ID of the newly selected showtime from the form
    [BindProperty]
    public int NewMovieTimeSlotId { get; set; }

    public PostponeModel(RazorPagesMovieContext context)
    {
        _context = context;
    }

    // --- OnGet (Display Current Booking & Available Slots) ---
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        // 1. Fetch the current booking details
        Booking = await _context.Booking
            .Include(b => b.MovieTimeSlot)
            .Include(b => b.MovieTimeSlot.Movie)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Booking == null) return NotFound();

        // 2. Determine the movie ID
        int currentMovieId = Booking.MovieTimeSlot.MovieId;
        int ticketsToMove = Booking.NumberOfTickets;

        // 3. Fetch all future showtimes for the same movie that have enough seats
        var availableSlots = await _context.MovieTimeSlot
            .Where(mts => mts.MovieId == currentMovieId &&
                          mts.Time > Booking.MovieTimeSlot.Time && // Must be a future time slot
                          mts.AvailableSeats >= ticketsToMove)     // Must have enough seats
            .OrderBy(mts => mts.Time)
            .ToListAsync();

        // 4. Populate the SelectList for the dropdown
        AvailableShowTimes = new SelectList(availableSlots, "Id", "Time");

        return Page();
    }

    // --- OnPost (Execute Postpone/Reschedule) ---
    public async Task<IActionResult> OnPostAsync(int id)
    {
        // 1. Fetch the original booking and its current showtime
        var originalBooking = await _context.Booking
            .Include(b => b.MovieTimeSlot)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (originalBooking == null) return NotFound();
        if (NewMovieTimeSlotId == 0) return Page(); // Handle no selection

        // 2. Fetch the newly selected showtime
        var newShowTime = await _context.MovieTimeSlot
            .FindAsync(NewMovieTimeSlotId);

        if (newShowTime == null)
        {
            ModelState.AddModelError("", "The selected new showtime is invalid or unavailable.");
            await OnGetAsync(id); // Reload the page with error message
            return Page();
        }

        // --- TRANSACTION START ---

        // 3. Update the OLD showtime (Return seats)
        originalBooking.MovieTimeSlot.AvailableSeats += originalBooking.NumberOfTickets;
        _context.MovieTimeSlot.Update(originalBooking.MovieTimeSlot);

        // 4. Update the NEW showtime (Take seats)
        newShowTime.AvailableSeats -= originalBooking.NumberOfTickets;
        _context.MovieTimeSlot.Update(newShowTime);

        // 5. Update the Booking record with the new MovieTimeSlotId
        originalBooking.MovieTimeSlotId = NewMovieTimeSlotId;
        _context.Booking.Update(originalBooking);

        // 6. Save all changes
        await _context.SaveChangesAsync();

        // --- TRANSACTION END ---

        // Redirect back to the main bookings list
        return RedirectToPage("./MyBookings");
    }
}