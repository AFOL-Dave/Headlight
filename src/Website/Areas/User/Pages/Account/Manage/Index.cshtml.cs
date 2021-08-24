using System.Threading.Tasks;
using Headlight.Areas.User.Models;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Areas.User.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public AccountDetails AccountDetails { get; set; }

        public IndexModel(UserManager<HeadLightUser> userManager,
                          SignInManager<HeadLightUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HeadLightUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            bool userChanged = false;

            if (AccountDetails.Country != user.Country)
            {
                user.Country = AccountDetails.Country;
                userChanged = true;
            }

            if (AccountDetails.StreetAddressLine1 != user.StreetAddressLine1)
            {
                user.StreetAddressLine1 = AccountDetails.StreetAddressLine1;
                userChanged = true;
            }

            if (AccountDetails.StreetAddressLine2 != user.StreetAddressLine2)
            {
                user.StreetAddressLine2 = AccountDetails.StreetAddressLine2;
                userChanged = true;
            }

            if (AccountDetails.Email != user.Email)
            {
                user.Email = AccountDetails.Email;
                user.EmailConfirmed = false;
                userChanged = true;
            }

            if (AccountDetails.City != user.City)
            {
                user.City = AccountDetails.City;
                userChanged = true;
            }

            if (AccountDetails.Region != user.Region)
            {
                user.Region = AccountDetails.Region;
                userChanged = true;
            }

            if (AccountDetails.PostalCode != user.PostalCode)
            {
                user.PostalCode = AccountDetails.PostalCode;
                userChanged = true;
            }

            if (AccountDetails.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = AccountDetails.PhoneNumber;
                user.PhoneNumberConfirmed = false;
                userChanged = true;
            }

            if (AccountDetails.GivenName != user.GivenName)
            {
                user.GivenName = AccountDetails.GivenName;
                userChanged = true;
            }

            if (AccountDetails.SurName != user.SurName)
            {
                user.SurName = AccountDetails.SurName;
                userChanged = true;
            }

            if (AccountDetails.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = AccountDetails.DateOfBirth;
                userChanged = true;
            }

            if (userChanged)
            {
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        private void Load(HeadLightUser user)
        {
            AccountDetails = new AccountDetails
            {
                City = user.City,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                DisplayName = user.DisplayName,
                Email = user.Email,
                GivenName = user.GivenName,
                PhoneNumber = user.PhoneNumber,
                SurName = user.SurName,
                PostalCode = user.PostalCode,
                Region = user.Region,
                StreetAddressLine1 = user.StreetAddressLine1,
                StreetAddressLine2 = user.StreetAddressLine2
            };
        }

        private readonly UserManager<HeadLightUser> _userManager;
        private readonly SignInManager<HeadLightUser> _signInManager;
    }
}