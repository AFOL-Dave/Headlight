using System.Text;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class EmailValidationModel : PageModel
    {
        public string ValidationMessage { get; set; }

        public EmailValidationModel(UserManager<HeadLightUser> userManager,
                                    ILogger<EmailValidationModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogWarning("An email validation was attempted which was missing either the userId or code.");
                ValidationMessage = "There was an error with your request.";
                return Page();
            }

            HeadLightUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("An email validation was attempted but the user was not found.");
                ValidationMessage = "There was an error retrieving your account.";
                return Page();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            ValidationMessage = result.Succeeded ? "Thank you for confirming your email. In order to fully engage, the LUG Administrator must approve your account." : "There was an error validatig your email.";
            return Page();
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly ILogger<EmailValidationModel> _logger;
    }
}