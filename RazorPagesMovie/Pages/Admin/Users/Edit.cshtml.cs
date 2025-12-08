using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity; // Essential for IdentityUser, RoleManager
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

// Define the namespace where your Admin pages reside
namespace RazorPagesMovie.Pages.Admin.Users
{
    // Place the helper class here, ensuring it is public and accessible
    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    [Authorize(Policy = "AdminPolicy")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 🌟 FIX 1: User property MUST be public for @Model.User.UserName to work.
        [BindProperty]
        public IdentityUser User { get; set; }

        // 🌟 FIX 2: UserId property MUST be public for @Model.UserId to work.
        // We make it public and non-nullable (string) here
        public string UserId { get; set; } = string.Empty;

        // 🌟 FIX 3: RoleList property MUST be public for @Model.RoleList[i] to work.
        [BindProperty]
        public List<RoleViewModel> RoleList { get; set; } = new List<RoleViewModel>();


        // --- Handlers ---

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            User = await _userManager.FindByIdAsync(id);
            if (User == null) return NotFound();

            // Set the public UserId property
            UserId = id;

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(User);

            RoleList = roles.Select(role => new RoleViewModel
            {
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            // The existing OnPost logic that handles role assignment...
            User = await _userManager.FindByIdAsync(userId);
            if (User == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(User);

            foreach (var role in RoleList)
            {
                if (role.IsSelected && !userRoles.Contains(role.RoleName))
                {
                    await _userManager.AddToRoleAsync(User, role.RoleName);
                }
                else if (!role.IsSelected && userRoles.Contains(role.RoleName))
                {
                    await _userManager.RemoveFromRoleAsync(User, role.RoleName);
                }
            }

            return RedirectToPage("./Index");
        }
    }
}