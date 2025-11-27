using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
        // This method runs when the page is first loaded (GET request)
    }

    public IActionResult OnPost()
    {
        // Simple static validation (replace with a database or real authentication logic)
        if (Username == "admin" && Password == "password123")
        {
            // If credentials are correct, store the username in the session
            HttpContext.Session.SetString("Username", Username); // Save username in session
            return RedirectToPage("/Index"); // Redirect to home page or another page
        }
        else
        {
            // If credentials are wrong, show error message
            ErrorMessage = "Invalid username or password.";
            return Page(); // Return the page with the error message
        }
    }
}
