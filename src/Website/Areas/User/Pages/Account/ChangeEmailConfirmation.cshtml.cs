using System.Text;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account
{
    public class ChangeEmailConfirmationModel : PageModel
    {
        public string ValidationMessage { get; set; }

        public ChangeEmailConfirmationModel(UserManager<HeadLightUser> userManager,
                                            SignInManager<HeadLightUser> signInManager,
                                            ILogger<ChangeEmailConfirmationModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                _logger.LogWarning("An email change validation was attempted which was missing either the userId, email or code.");
                ValidationMessage = "There was an error with your request.";
                return Page();
            }

            HeadLightUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("An email change validation was attempted but the user was not found.");
                ValidationMessage = "There was an error retrieving your account.";
                return Page();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await _userManager.ChangeEmailAsync(user, email, code);

            if (!result.Succeeded)
            {
                ValidationMessage = "There was an error changing your email.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            ValidationMessage = "Thank you for confirming your email change.";
            return Page();
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly SignInManager<HeadLightUser> _signInManager;
        ILogger<ChangeEmailConfirmationModel> _logger;
    }
}