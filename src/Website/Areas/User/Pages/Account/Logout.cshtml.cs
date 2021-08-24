using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        public LogoutModel(SignInManager<HeadLightUser> signInManager,
                           ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }

        private readonly SignInManager<HeadLightUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
    }
}