using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Admin
{
    [Authorize(Roles = "Admin")] // Admin only
    public class AddMovieModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public AddMovieModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AddMovie"); // stay on same page or redirect to list
        }
    }
}
