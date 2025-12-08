using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq; // For the ToList() method
using System.Threading.Tasks;

// 1. Ensure the class name matches your @model declaration in the HTML
// (I will use UsersIndexModel, assuming you will change your @model to match)
[Authorize(Policy = "AdminPolicy")]
public class UsersIndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    // 🌟 THE FIX: This public property MUST be declared and spelled correctly
    public IList<IdentityUser> Users { get; set; }

    public UsersIndexModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public void OnGet() // We can use synchronous OnGet for this simple list
    {
        // 2. Load all users into the public property
        // We use ToList() here to execute the query immediately
        Users = _userManager.Users.ToList();
    }
}