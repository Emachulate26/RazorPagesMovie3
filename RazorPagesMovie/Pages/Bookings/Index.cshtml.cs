using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; } = default!;
        public string? UserName { get; set; }

        public string MovieTitle { get; set; }
        public DateTime ShowTime { get; set; }

        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }
        public decimal Price { get; set; }


        public async Task OnGetAsync()
        {
            if (_context.Booking != null)
            {
                // CRITICAL FIX: Use ThenInclude to load the Movie associated with the MovieTimeSlot
                Booking = await _context.Booking
                    .Include(b => b.MovieTimeSlot)
                        .ThenInclude(mts => mts.Movie) // <-- This loads the Movie details!
                    .ToListAsync();
            }
        }
    }
}