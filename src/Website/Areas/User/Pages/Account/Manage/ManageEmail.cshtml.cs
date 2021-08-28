using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Headlight.Areas.User.Pages.Account.Manage
{
    public class ManageEmailModel : PageModel
    {
        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        [EmailAddress]
        public string NewEmail { get; set; }

        public string OldEmail { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public ManageEmailModel(UserManager<HeadLightUser> userManager,
                                SignInManager<HeadLightUser> signInManager,
                                IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
                await _emailSender.SendEmailAsync(NewEmail,
                                                  "Confirm Your New Email",
                                                  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
            await _emailSender.SendEmailAsync(email,
                                              "Confirm Your Email",
                                              $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
        private readonly SignInManager<HeadLightUser> _signInManager;
        private readonly IEmailSender _emailSender;
    }
}