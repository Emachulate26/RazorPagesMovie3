

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

                var mallOfAfrica = context.Cinema.FirstOrDefault(c => c.Name == "NuMetro Mall of Africa");

                // --- 2. SEED MOVIES (Adapted to match the simplified Movie.cs model) ---
                if (!context.Movie.Any())
                {
                    context.Movie.AddRange(
                        new Movie
                        {
                            Title = "When Harry Met Sally",
                            Description = "A classic romantic comedy.",
                            TotalPrice = 7.99M
                        },
                        new Movie
                        {
                            Title = "Ghostbusters",
                            Description = "Who you gonna call?",
                            TotalPrice = 8.99M
                        },
                        new Movie
                        {
                            Title = "John Wick",
                            Description = "He wants his dog back.",
                            TotalPrice = 12.99M
                        },
                        new Movie
                        {
                            Title = "Avata",
                            Description = "Epic sci-fi adventure.",
                            TotalPrice = 13.99M
                        }
                    );
                    context.SaveChanges(); // SAVE MOVIES FIRST to generate their IDs!
                }

                // --- 3. SEED MOVIE TIME SLOTS ---
                if (context.MovieTimeSlot.Any())
                {
                    return;
                }

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
                            CinemaId = mallOfAfrica.Id,
                            // CORRECTED: StartTime changed to Time
                            Time = DateTime.Today.AddDays(1).AddHours(18).AddMinutes(0), // Tomorrow 6:00 PM
                            ScreenNumber = 1, // Added ScreenNumber property (assumed value)
                            AvailableSeats = 150
                        },
                        // Postpone Target Slot (Future time for the same movie)
                        new MovieTimeSlot
                        {
                            MovieId = johnWick.Id,
                            CinemaId = mallOfAfrica.Id,
                            // CORRECTED: StartTime changed to Time
                            Time = DateTime.Today.AddDays(2).AddHours(21).AddMinutes(30), // Day After Tomorrow 9:30 PM
                            ScreenNumber = 2, // Added ScreenNumber property (assumed value)
                            AvailableSeats = 150
                        },
                        // Avata Showtime
                        new MovieTimeSlot
                        {
                            MovieId = avata.Id,
                            CinemaId = mallOfAfrica.Id,
                            // CORRECTED: StartTime changed to Time
                            Time = DateTime.Today.AddDays(2).AddHours(14).AddMinutes(0), // Day after 2:00 PM
                            ScreenNumber = 3, // Added ScreenNumber property (assumed value)
                            AvailableSeats = 200
                        },
                        // When Harry Met Sally Showtime
                        new MovieTimeSlot
                        {
                            MovieId = harrySally.Id,
                            CinemaId = mallOfAfrica.Id,
                            // CORRECTED: StartTime changed to Time
                            Time = DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), // Tomorrow 4:30 PM
                            ScreenNumber = 4, // Added ScreenNumber property (assumed value)
                            AvailableSeats = 100
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}