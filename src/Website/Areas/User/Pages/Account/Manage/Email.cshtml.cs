using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Headlight.Models;
using Headlight.Models.Options;
using Headlight.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Headlight.Areas.User.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        [EmailAddress]
        public string NewEmail { get; set; }

        public string OldEmail { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public EmailModel( UserManager<HeadLightUser> userManager,
                           IEmailService emailService,
                           IOptions<LugOptions> lugOptions)
        {
            _userManager = userManager;
            _emailService = emailService;
            _lugOptions = lugOptions.Value;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                StatusMessage = "There was an error loading your account.";
                return RedirectToPage();
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                StatusMessage = "There was an error loading your account.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            string email = await _userManager.GetEmailAsync(user);

            if (NewEmail != email)
            {
                string userId = await _userManager.GetUserIdAsync(user);
                string code = await _userManager.GenerateChangeEmailTokenAsync(user, NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string callbackUrl = Url.Page("/Account/ChangeEmailConfirmation",
                                              pageHandler: null,
                                              values: new { userId = userId, email = NewEmail, code = code },
                                              protocol: Request.Scheme);
                
                IEmailAddress sender = new EmailAddress{ Name = _lugOptions.FullName, Address = _lugOptions.ApproverEmail };
                IEmailAddress recipient = new EmailAddress{ Name = user.DisplayName, Address = NewEmail };
                await _emailService.SendSingleEmailAsync(sender, recipient, "Confirm Your Email Address",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                    $"Please confirm your account by visiting this link: {HtmlEncoder.Default.Encode(callbackUrl)}.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                StatusMessage = "There was an error loading your account.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            string userId = await _userManager.GetUserIdAsync(user);
            string email = await _userManager.GetEmailAsync(user);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string callbackUrl = Url.Page("/Account/EmailConfirmation",
                                          pageHandler: null,
                                          values: new { area = "User", userId = userId, code = code },
                                          protocol: Request.Scheme);

            IEmailAddress sender = new EmailAddress{ Name = _lugOptions.FullName, Address = _lugOptions.ApproverEmail };
            IEmailAddress recipient = new EmailAddress{ Name = user.DisplayName, Address = user.Email };
            await _emailService.SendSingleEmailAsync(sender, recipient, "Confirm Your Email Address",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                $"Please confirm your account by visiting this link: {HtmlEncoder.Default.Encode(callbackUrl)}.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }

        private async Task LoadAsync(HeadLightUser user)
        {
            string email = await _userManager.GetEmailAsync(user);

            NewEmail = email;
            OldEmail = email;

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly LugOptions _lugOptions;
    }
}