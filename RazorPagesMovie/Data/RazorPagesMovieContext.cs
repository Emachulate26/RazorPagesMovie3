using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Data
{
    public class RazorPagesMovieContext : IdentityDbContext
    {
        public RazorPagesMovieContext(DbContextOptions<RazorPagesMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
        // Add more DbSets if needed:
        // public DbSet<TimeSlot> TimeSlots { get; set; }
        // public DbSet<Booking> Bookings { get; set; }
    }
}
