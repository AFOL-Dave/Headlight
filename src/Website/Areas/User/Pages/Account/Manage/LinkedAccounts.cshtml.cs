using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account.Manage
{
    public class LinkedAccountsModel : PageModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public LinkedAccountsModel( UserManager<HeadLightUser> userManager,
                                    SignInManager<HeadLightUser> signInManager,
                                    ILogger<LinkedAccountsModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("There was an error loading an account.");
                StatusMessage = "Error: There:  was an error loading your account.";
                return RedirectToPage();
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = CurrentLogins.Count > 1;

            return Page();
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("There was an error loading an account.");
                StatusMessage = "Error: There was an error loading your account.";
                return RedirectToPage();
            }

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());

            if (info == null)
            {
                _logger.LogWarning($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
                StatusMessage = "Error: There was an error loading your account.";
                return RedirectToPage();
            }

            IdentityResult result = await _userManager.AddLoginAsync(user, info);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: The external login was not added. External logins can only be associated with one account.";
                return RedirectToPage();
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./ManageLinkedAccounts", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning("There was an error loading an account.");
                StatusMessage = "Error: There was an error loading your account.";
                return RedirectToPage();
            }

            IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: The external login was not removed.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly SignInManager<HeadLightUser> _signInManager;
        private readonly ILogger<LinkedAccountsModel> _logger;
    }
}