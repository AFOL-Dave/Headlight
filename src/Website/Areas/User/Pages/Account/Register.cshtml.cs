using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        public IList<AuthenticationScheme> IdentityProviders { get; set; }

        public string ReturnUrl { get; set; }

        public RegisterModel(SignInManager<HeadLightUser> signInManager,
                             ILogger<RegisterModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            IdentityProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if ((IdentityProviders?.Count ?? 0) > 0)
            {
                foreach(AuthenticationScheme provider in IdentityProviders)
                {
                    _logger.LogInformation($"{provider.DisplayName} available as an external identity provider.");
                }
            }
        }

        private readonly SignInManager<HeadLightUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
    }
}