using System.Text;
using System.Threading.Tasks;
using Headlight.Models;
using Headlight.Models.Options;
using Headlight.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class EmailConfirmationModel : PageModel
    {
        public string ValidationMessage { get; set; }

        public EmailConfirmationModel( UserManager<HeadLightUser> userManager,
                                       IEmailService emailService,
                                       IOptions<LugOptions> lugOptions,
                                       ILogger<EmailConfirmationModel> logger )
        {
            _userManager = userManager;
            _emailService = emailService;
            _lugOptions = lugOptions.Value;
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
            IEmailAddress sender = new EmailAddress { Name = _lugOptions.FullName, Address = _lugOptions.ApproverEmail };
            IEmailAddress recipient = new EmailAddress { Name = _lugOptions.FullName, Address = _lugOptions.ApproverEmail };
            _emailService.SendSingleEmailAsync(sender, recipient, "New User Registration", "A new user has registered.", "A new user has registered.");
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly ILogger<EmailConfirmationModel> _logger;
        private readonly IEmailService _emailService;
        private readonly LugOptions _lugOptions;
    }
}