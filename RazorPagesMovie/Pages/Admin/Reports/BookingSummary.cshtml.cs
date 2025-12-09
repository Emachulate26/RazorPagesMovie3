using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// Place the ReportItem class here to ensure it's uniquely accessible
public class ReportItem
{
    public string MovieTitle { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
}

namespace RazorPagesMovie.Pages.Admin.Reports
{
    [Authorize(Policy = "AdminPolicy")]
    public class BookingSummaryModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public BookingSummaryModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<ReportItem> BookingSummary { get; set; } = new List<ReportItem>();

        public async Task OnGetAsync()
        {
            // The query is much simpler as we only query the Booking table!
            var adjustedEndDate = EndDate.Date.AddDays(1).AddSeconds(-1);

            BookingSummary = await _context.Booking
                // 1. FILTER: By BookingDate
                .Where(b => b.BookingDate.Date >= StartDate.Date &&
                            b.BookingDate.Date <= adjustedEndDate.Date)

                // 2. GROUP: By the MovieTitle already present on the Booking record
                .GroupBy(b => b.MovieTitle)

                // 3. SELECT: Calculate the totals
                .Select(g => new ReportItem
                {
                    MovieTitle = g.Key,
                    TotalBookings = g.Sum(b => b.NumberOfTickets),
                    // 🌟 FIX: Calculate revenue by multiplying Price * NumberOfTickets
                    TotalRevenue = g.Sum(b => b.NumberOfTickets * b.TotalPrice)
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToListAsync();
        }
    }
}