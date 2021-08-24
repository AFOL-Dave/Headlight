using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class RegistrationConfirmationModel : PageModel
    {
        public RegistrationConfirmationModel(UserManager<HeadLightUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            HeadLightUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            return Page();
        }

        private readonly UserManager<HeadLightUser> _userManager;
    }
}