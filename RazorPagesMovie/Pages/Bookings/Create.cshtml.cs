using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using RazorPagesMovie.Models;
// NOTE: Make sure the RazorPagesMovie.Data namespace is correct for your DbContext

namespace RazorPagesMovie.Pages.Bookings
{
    // Authorize the page so only logged-in users can book
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Fetch available Movie Time Slots for the dropdown
            // (Existing logic remains the same)
            ViewData["MovieTimeSlotId"] = new SelectList(
                await _context.MovieTimeSlot
                    .Include(mts => mts.Movie)
                    .Select(mts => new
                    {
                        Id = mts.Id,
                        DisplayText = $"{mts.Movie.Title} - {mts.StartTime:yyyy-MM-dd HH:mm}"
                    })
                    .ToListAsync(),
                "Id",
                "DisplayText");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Check if the model state is valid (e.g., if a slot and ticket count were provided)
            // We use TryValidateModel to ensure only the necessary fields are validated if the 
            // model state includes fields we intend to set later (like Price).
            if (!ModelState.IsValid)
            {
                // Re-populate ViewData if validation fails
                // (Existing re-population logic remains the same)
                ViewData["MovieTimeSlotId"] = new SelectList(
                    await _context.MovieTimeSlot
                        .Include(mts => mts.Movie)
                        .Select(mts => new
                        {
                            Id = mts.Id,
                            DisplayText = $"{mts.Movie.Title} - {mts.StartTime:yyyy-MM-dd HH:mm}"
                        })
                        .ToListAsync(),
                    "Id",
                    "DisplayText");
                return Page();
            }

            // --- ADDED LOGIC STARTS HERE ---

            // Step 2: Fetch the selected MovieTimeSlot to get the ticket price
            var timeSlot = await _context.MovieTimeSlot
                // Assuming your MovieTimeSlot has a Price or links to a Movie which has a Price
                .Include(mts => mts.Movie)
                .FirstOrDefaultAsync(mts => mts.Id == Booking.MovieTimeSlotId);

            if (timeSlot == null)
            {
                // Add a model error if the time slot is invalid
                ModelState.AddModelError("Booking.MovieTimeSlotId", "The selected show time is not valid.");
                return Page(); // Re-show the page with the error
            }

            // Step 3: Automatically assign the required Booking properties:

            // A. Assign logged-in User's ID and Name
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Booking.UserId = user.Id;
                Booking.UserName = user.Email; // Use email as a simple display name
            }

            // B. Set the booking date to the current date/time
            Booking.BookingDate = DateTime.Now;

            // C. Calculate the total Price
            // ASSUMPTION: The unit price is stored on the Movie model (timeSlot.Movie.Price)
            // You may need to adjust the property name based on your actual Movie/MovieTimeSlot model structure
            // We'll use a placeholder variable if the price isn't readily available for calculation.
            // Example: decimal unitPrice = timeSlot.Movie.TicketPrice; 

            // Using a simple placeholder price for demonstration:
            decimal unitPrice = timeSlot.Movie.Price > 0 ? timeSlot.Movie.Price : 12.50m; // Use movie price or default

            Booking.Price = Booking.NumberOfTickets * unitPrice;

            // --- ADDED LOGIC ENDS HERE ---

            // Step 4: Save the Booking
            _context.Booking.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}