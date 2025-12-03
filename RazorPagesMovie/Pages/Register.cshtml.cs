using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Models;
using RazorPagesMovie.Services;

namespace RazorPagesMovie.Pages
{
    public class RegisterModelPage : PageModel
    {
        private readonly UserService _userService;

        public RegisterModelPage(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterModel Input { get; set; } = new RegisterModel();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _userService.AddUser(Input.Username, Input.Password);

            return RedirectToPage("/Index");
        }
    }
}
