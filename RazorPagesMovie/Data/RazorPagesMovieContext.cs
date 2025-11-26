using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Data
{
    public class RazorPagesMovieContext : DbContext
    {
        public RazorPagesMovieContext(DbContextOptions<RazorPagesMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; } = default!;

        // ADD THESE FOR BOOKINGS SYSTEM
        public DbSet<Cinema> Cinema { get; set; }
        public DbSet<MovieTimeSlot> MovieTimeSlot { get; set; }
        public DbSet<Booking> Booking { get; set; }
    }
}
