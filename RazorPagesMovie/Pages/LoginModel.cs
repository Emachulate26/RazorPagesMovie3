using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

public class LoginModel : PageModel
{
    // Define the username and password as nullable properties
    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? Password { get; set; }

    // Define ErrorMessage to store any error messages
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // This is called when the page is first loaded (GET request)
    }

    public IActionResult OnPost()
    {
        // Simple static validation (replace with real logic in a real app)
        if (Username == "admin" && Password == "password123")
        {
            // Store the username in the session on successful login
            HttpContext.Session.SetString("Username", Username!);  // Store username in session
            return RedirectToPage("/Index");  // Redirect to home page after successful login
        }
        else
        {
            // If credentials are wrong, show error message
            ErrorMessage = "Invalid username or password.";  // Set error message
            return Page();  // Return the page with error message
        }
    }
}
