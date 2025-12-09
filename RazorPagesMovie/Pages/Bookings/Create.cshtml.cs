

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    private readonly RazorPagesMovieContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    // This will hold the data submitted from the form (Booking details)
    [BindProperty]
    public Booking Booking { get; set; } = new Booking();

    // This is used to populate the dropdown list in the view
    public SelectList ShowTimeOptions { get; set; } = default!;

    public CreateModel(RazorPagesMovieContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // --- OnGet (Display Form) ---
    public async Task<IActionResult> OnGetAsync()
    {
        // Fetch all showtimes happening in the future
        var availableShowTimes = await _context.MovieTimeSlot
            .Include(mts => mts.Movie)
            .Include(mts => mts.Cinema)
            .Where(mts => mts.Time > DateTime.Now)
            .OrderBy(mts => mts.Time)
            .ToListAsync();

        // Format the display text for the user (e.g., "Movie Title - 10/Dec @ 8:00 PM (Screen 3)")
        var formattedOptions = availableShowTimes.Select(mts => new
        {
            Id = mts.Id,
            Display = $"{mts.Movie.Title} - {mts.Time.ToString("dd MMM @ hh:mm tt")} (Screen {mts.ScreenNumber} at {mts.Cinema.Name})"
        }).ToList();

        // Populate the SelectList
        ShowTimeOptions = new SelectList(formattedOptions, "Id", "Display");

        return Page();
    }

    // --- OnPost (Handle Submission) ---
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(); // Reload options if validation fails
            return Page();
        }

        // 1. Fetch the selected showtime and its movie details
        var selectedShowTime = await _context.MovieTimeSlot
            .Include(mts => mts.Movie)
            .FirstOrDefaultAsync(mts => mts.Id == Booking.MovieTimeSlotId);

        if (selectedShowTime == null || selectedShowTime.AvailableSeats < Booking.NumberOfTickets)
        {
            ModelState.AddModelError("", "Selected showtime is invalid or does not have enough available seats.");
            await OnGetAsync();
            return Page();
        }

        // --- TRANSACTION START ---

        // 2. Calculate Total Price
        Booking.TotalPrice = Booking.NumberOfTickets * selectedShowTime.Movie.TotalPrice;

        // 3. Assign User ID and Booking Date
        Booking.UserId = _userManager.GetUserId(User);
        Booking.BookingDate = DateTime.Now;

        // 4. Update the Showtime's available seats (decrement)
        selectedShowTime.AvailableSeats -= Booking.NumberOfTickets;
        _context.MovieTimeSlot.Update(selectedShowTime);

        // 5. Add the new Booking
        _context.Booking.Add(Booking);

        // 6. Save changes
        await _context.SaveChangesAsync();

        // --- TRANSACTION END ---

        return RedirectToPage("./MyBookings");
    }
}
