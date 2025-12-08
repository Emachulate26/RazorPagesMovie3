using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System;
using System.Linq;

namespace RazorPagesMovie.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RazorPagesMovieContext(
                serviceProvider.GetRequiredService<DbContextOptions<RazorPagesMovieContext>>()))
            {
                if (context == null || context.Movie == null)
                {
                    throw new ArgumentNullException("Null RazorPagesMovieContext");
                }

                // --- 1. SEED CINEMA (REQUIRED to get a valid CinemaId) ---
                if (!context.Cinema.Any())
                {
                    context.Cinema.AddRange(
                        new Cinema { Name = "NuMetro Mall of Africa" },
                        new Cinema { Name = "NuMetro Hyde Park" }
                    );
                    context.SaveChanges();
                }

                // Get the ID of a seeded Cinema for linking showtimes
                var mallOfAfrica = context.Cinema.FirstOrDefault(c => c.Name == "NuMetro Mall of Africa");

                // --- 2. SEED MOVIES ---
                if (!context.Movie.Any())
                {
                    context.Movie.AddRange(
                        // Movies from your screenshot
                        new Movie { Title = "When Harry Met Sally", ReleaseDate = DateTime.Parse("1989-2-12"), Genre = "Romantic Comedy", Price = 7.99M, Rating = "R" },
                        new Movie { Title = "Ghostbusters", ReleaseDate = DateTime.Parse("1984-3-13"), Genre = "Comedy", Price = 8.99M, Rating = "R" },
                        new Movie { Title = "Ghostbusters 2", ReleaseDate = DateTime.Parse("1986-2-23"), Genre = "Comedy", Price = 9.99M, Rating = "R" },
                        new Movie { Title = "Rio Bravo", ReleaseDate = DateTime.Parse("1959-4-15"), Genre = "Western", Price = 3.99M, Rating = "R" },
                        new Movie { Title = "John Wick", ReleaseDate = DateTime.Parse("2014-10-24"), Genre = "Action", Price = 12.99M, Rating = "R" },
                        new Movie { Title = "Avata", ReleaseDate = DateTime.Parse("2009-12-18"), Genre = "Action", Price = 13.99M, Rating = "PG-13" },
                        new Movie { Title = "Iron heart", ReleaseDate = DateTime.Parse("2023-11-10"), Genre = "Action", Price = 14.99M, Rating = "PG-13" }
                    );
                    context.SaveChanges(); // SAVE MOVIES FIRST to generate their IDs!
                }

                // --- 3. SEED MOVIE TIME SLOTS ---
                if (context.MovieTimeSlot.Any())
                {
                    return; // DB has already been seeded with showtimes
                }

                // IMPORTANT: Query the Movie and Cinema IDs back to establish the relationships
                var johnWick = context.Movie.FirstOrDefault(m => m.Title == "John Wick");
                var avata = context.Movie.FirstOrDefault(m => m.Title == "Avata");
                var harrySally = context.Movie.FirstOrDefault(m => m.Title == "When Harry Met Sally");

                if (mallOfAfrica != null && johnWick != null && avata != null && harrySally != null)
                {
                    context.MovieTimeSlot.AddRange(
                        // John Wick Showtimes
                        new MovieTimeSlot
                        {
                            MovieId = johnWick.Id,
                            CinemaId = mallOfAfrica.Id, // CORRECTED COLUMN NAME
                            StartTime = DateTime.Today.AddDays(1).AddHours(18).AddMinutes(0), // Tomorrow 6:00 PM
                            AvailableSeats = 150
                        },
                        new MovieTimeSlot
                        {
                            MovieId = johnWick.Id,
                            CinemaId = mallOfAfrica.Id,
                            StartTime = DateTime.Today.AddDays(1).AddHours(21).AddMinutes(30), // Tomorrow 9:30 PM
                            AvailableSeats = 150
                        },
                        // Avata Showtimes
                        new MovieTimeSlot
                        {
                            MovieId = avata.Id,
                            CinemaId = mallOfAfrica.Id,
                            StartTime = DateTime.Today.AddDays(2).AddHours(14).AddMinutes(0), // Day after 2:00 PM
                            AvailableSeats = 200
                        },
                        // When Harry Met Sally Showtime
                        new MovieTimeSlot
                        {
                            MovieId = harrySally.Id,
                            CinemaId = mallOfAfrica.Id,
                            StartTime = DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), // Tomorrow 4:30 PM
                            AvailableSeats = 100
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}