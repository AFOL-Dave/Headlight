using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Headlight.Areas.User.Models;
using Headlight.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        [TempData]
        public string ErrorMessage {get; set;}

        public string ProviderDisplayName { get; set; }

        [BindProperty]
        public Registration Registration {get; set;}

        public string ReturnUrl {get; set;}

        public ExternalLoginModel(SignInManager<HeadLightUser> signInManager,
                                  UserManager<HeadLightUser> userManager,
                                  ILookupNormalizer normalizer,
                                  IEmailSender emailSender,
                                  ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _normalizer = normalizer;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        /// <summary>
        ///  Handles the response received from the external identity provider.
        /// </summary>
        /// <param name="returnUrl">
        /// </param>
        /// <param name="remoteError">
        /// </param>
        /// <returns>
        /// </returns>
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                ErrorMessage = "Error loading extenral login infomration.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation($"{loginInfo.Principal?.Identity?.Name} logged in with {loginInfo.LoginProvider} provider.");
                return LocalRedirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                _logger.LogInformation($"{loginInfo.Principal?.Identity?.Name} was blocked from logging in, because they are locked out.");
                return RedirectToPage("./Lockout");
            }

            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = loginInfo.ProviderDisplayName;

            if (loginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Registration = new Registration
                {
                    Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                    DisplayName = loginInfo.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }

            return Page();
        }

        /// <summary>
        ///  Configures and makes the call to the external identity provider.
        /// </summary>
        /// <param name="provider">
        /// </param>
        /// <param name="returnUrl">
        /// </param>
        /// <returns>
        /// </returns>
        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            string redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                ErrorMessage = "Error loading extenral login infomration.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            if (ModelState.IsValid)
            {
                HeadLightUser newUser = new HeadLightUser
                {
                    Id = -1,
                    DateOfBirth = Registration.DateOfBirth,
                    DisplayName = Registration.DisplayName,
                    Email = Registration.Email,
                    GivenName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                    IsActive = true,
                    NormalizedEmail = _normalizer.NormalizeEmail(Registration.Email),
                    NormalizedUserName = _normalizer.NormalizeEmail(Registration.Email),
                    SurName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname),
                    UserName = Registration.Email
                };

                IdentityResult result = await _userManager.CreateAsync(newUser);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(newUser, loginInfo);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User created an account using {loginInfo.LoginProvider} provider.");

                        string userId = await _userManager.GetUserIdAsync(newUser);
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        string callbackUrl = Url.Page("/Account/EmailValidation",
                                                      pageHandler: null,
                                                      values: new { area = "User", userId, code },
                                                      protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Registration.Email, "Confirm Your Email Address",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegistrationConfirmation", new { Registration.Email });
                        }

                        await _signInManager.SignInAsync(newUser, isPersistent: false, loginInfo.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = loginInfo.ProviderDisplayName;
            ReturnUrl = returnUrl;

            return Page();
        }

        private readonly SignInManager<HeadLightUser> _signInManager;
        private readonly UserManager<HeadLightUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly ILookupNormalizer _normalizer;
        private readonly IEmailSender _emailSender;
    }
}