using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Define the correct namespace
namespace RazorPagesMovie.Pages.Admin.Users
{
    [Authorize(Policy = "AdminPolicy")]
    // Ensure the class name is UsersIndexModel to avoid conflicts
    public class UsersIndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // <-- Required for role checking

        public IList<IdentityUser> Users { get; set; } = new List<IdentityUser>();
        public IList<IdentityUser> Admins { get; set; } = new List<IdentityUser>(); // <-- List of Admin users

        public UsersIndexModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task OnGet() // Must be async
        {
            // 1. Load the full list of all users
            Users = _userManager.Users.ToList();

            // 2. Load users in the "Admin" role
            // This method is the safest way to get users by role.
            var adminUsersCollection = await _userManager.GetUsersInRoleAsync("Admin");

            // Convert the ICollection to IList<IdentityUser> for the view property
            Admins = adminUsersCollection.ToList();
        }
    }
}