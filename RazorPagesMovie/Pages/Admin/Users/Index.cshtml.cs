// Pages/Admin/Users/Index.cshtml.cs
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
    // 🌟 FIX: Use UsersIndexModel to resolve CS0229 Ambiguity
    public class UsersIndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IList<IdentityUser> Users { get; set; }

        public UsersIndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}