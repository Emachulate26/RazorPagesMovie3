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

        // -----------------------------------------------------------
        // 1. UPDATED: No more ViewData population on initial GET
        // -----------------------------------------------------------
        public IActionResult OnGet()
        {
            return Page();
        }

        // -----------------------------------------------------------
        // 2. NEW HANDLER: AJAX Endpoint for Typeahead/Search
        // -----------------------------------------------------------
        public async Task<JsonResult> OnGetSearchTimeSlotsAsync(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 3)
            {
                // Require at least 3 characters to start searching
                return new JsonResult(new List<object>());
            }
            string searchTerm = term.ToLower();

            var timeSlots = await _context.MovieTimeSlot
                .Include(mts => mts.Movie)
                .Where(mts => mts.Movie.Title.ToLower().Contains(searchTerm))
                .OrderBy(mts => mts.Movie.Title)
                .ThenBy(mts => mts.StartTime)
                .Take(20) // Limit the number of results
                .Select(mts => new
                {
                    id = mts.Id,
                    // Format the text to show both Movie Title and Show Time
                    text = $"{mts.Movie.Title} - {mts.StartTime:yyyy-MM-dd HH:mm}"
                })
                .ToListAsync();

            return new JsonResult(timeSlots);
        }

        // -----------------------------------------------------------
        // 3. ONPOST: Logic to save the booking
        // -----------------------------------------------------------
        public async Task<IActionResult> OnPostAsync()
        {
            // We use TryValidateModel to ensure only the necessary fields are validated 
            // before we manually set BookingDate and Price.
            if (!ModelState.IsValid)
            {
                // If validation fails, return Page()
                return Page();
            }

            // Step 2: Fetch the selected MovieTimeSlot to get the ticket price
            var timeSlot = await _context.MovieTimeSlot
                .Include(mts => mts.Movie)
                .FirstOrDefaultAsync(mts => mts.Id == Booking.MovieTimeSlotId);

            if (timeSlot == null)
            {
                ModelState.AddModelError("Booking.MovieTimeSlotId", "The selected show time is not valid.");
                return Page();
            }

            // Step 3: Automatically assign the required Booking properties:

            // A. Assign logged-in User's ID and Name
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Booking.UserId = user.Id;
                Booking.UserName = user.Email;
            }

            // B. Set the booking date to the current date/time
            Booking.BookingDate = DateTime.Now;

            // C. Calculate the total Price
            // ASSUMPTION: The unit price is stored on the Movie model (timeSlot.Movie.Price)
            decimal unitPrice = timeSlot.Movie.Price > 0 ? timeSlot.Movie.Price : 12.50m;
            Booking.Price = Booking.NumberOfTickets * unitPrice;
            // Step 4: Save the Booking
            _context.Booking.Add(Booking); // Marks the new booking for insertion
            await _context.SaveChangesAsync(); // Executes the insert command to the database!

            return RedirectToPage("./Index");
        }
    }
}