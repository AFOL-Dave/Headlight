using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Headlight.Areas.User.Models;
using Headlight.Models;
using Headlight.Models.Enumerations;
using Headlight.Models.Options;
using Headlight.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public ExternalLoginModel( SignInManager<HeadLightUser> signInManager,
                                   UserManager<HeadLightUser> userManager,
                                   HeadLightMembershipStore membershipStore,
                                   HeadLightUserGroupStore userGroupStore,
                                   ILookupNormalizer normalizer,
                                   IEmailService emailService,
                                   IOptions<LugOptions> lugOptions,
                                   ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _membershipStore = membershipStore;
            _userGroupStore = userGroupStore;
            _emailService = emailService;
            _normalizer = normalizer;
            _lugOptions = lugOptions.Value;
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
                        HeadLightUserGroup userGroup = await _userGroupStore.RetrieveUserGroupAsync();

                        HeadLightMembership membership = new HeadLightMembership()
                        {
                            UserId = newUser.Id,
                            UserGroupId = userGroup.Id,
                            IsActive = userGroup.JoinType != UserGroupJoinType.Gated
                        };

                        await _membershipStore.CreateMembershipAsync(membership);

                        _logger.LogInformation($"User created an account using {loginInfo.LoginProvider} provider.");

                        string userId = await _userManager.GetUserIdAsync(newUser);
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        string callbackUrl = Url.Page("/Account/EmailConfirmation",
                                                      pageHandler: null,
                                                      values: new { area = "User", userId, code },
                                                      protocol: Request.Scheme);

                        IEmailAddress sender = new EmailAddress{ Name = _lugOptions.FullName, Address = _lugOptions.ApproverEmail };
                        IEmailAddress recipient = new EmailAddress{ Name = newUser.DisplayName, Address = newUser.Email };
                        await _emailService.SendSingleEmailAsync(sender, recipient, "Confirm Your Email Address",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                            $"Please confirm your account by visiting this link: {HtmlEncoder.Default.Encode(callbackUrl)}.");

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
        private readonly HeadLightMembershipStore _membershipStore;
        private readonly HeadLightUserGroupStore _userGroupStore;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly ILookupNormalizer _normalizer;
        private readonly IEmailService _emailService;
        private readonly LugOptions _lugOptions;
    }
}