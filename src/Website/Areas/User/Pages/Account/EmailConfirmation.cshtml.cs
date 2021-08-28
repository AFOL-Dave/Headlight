using System.Text;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class EmailConfirmationModel : PageModel
    {
        public string ValidationMessage { get; set; }

        public EmailConfirmationModel(UserManager<HeadLightUser> userManager,
                                      IEmailSender emailSender,
                                      ILogger<EmailConfirmationModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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

            // Capture if the user has previously confirmed their email address, so that the administrator is not spammed everytime the user clicks the link in their email.
            bool WasEmailPreviouslyConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                ValidationMessage = "Thank you for confirming your email. In order to fully engage, the LUG Administrator must approve your account.";

                if (!WasEmailPreviouslyConfirmed)
                {
                    SendAdministratorEmail();
                }
            }
            else
            {
                ValidationMessage = "There was an error validatig your email.";
            }

            return Page();
        }

        private void SendAdministratorEmail()
        {
            _emailSender.SendEmailAsync("info@kenshalug.org",
                                        "New Registration",
                                        $"A new user has registered.");
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly ILogger<EmailConfirmationModel> _logger;
        private readonly IEmailSender _emailSender;
    }
}