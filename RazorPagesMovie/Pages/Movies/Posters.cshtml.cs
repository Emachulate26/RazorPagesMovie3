using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesMovie.Pages.Movies
{
    public class PostersModel : PageModel
    {
        public List<string> Posters { get; set; } = new List<string>();

        public void OnGet()
        {
            Posters = new List<string>
            {
                "/images/familyplan2.jpg",
                "/images/altered.jpg",
                "/images/frankenstein.jpg",
                "/images/alegend.jpg",
                "/images/wicked.jpg"
            };
        }
    }
}
