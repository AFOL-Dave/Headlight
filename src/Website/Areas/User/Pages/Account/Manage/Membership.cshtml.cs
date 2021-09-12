using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.User.Pages.Account.Manage
{
    public class MembershipModel : PageModel
    {
        [BindProperty]
        public HeadLightMembership MembershipDetails {get; set;}

        [TempData]
        public string StatusMessage { get; set; }

        public MembershipModel( HeadLightMembershipStore membershipStore,
                                UserManager<HeadLightUser> userManager,
                                ILogger<MembershipModel> logger )
        {
            _membershipStore = membershipStore;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            MembershipDetails = await Load(User);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HeadLightMembership membership = await Load(User);

            if (!ModelState.IsValid)
            {
                MembershipDetails = membership;
                return Page();
            }

            bool membershipChanged = false;

            if (MembershipDetails.IsPrimary != membership.IsPrimary)
            {
                membership.IsPrimary = MembershipDetails.IsPrimary;
                membershipChanged = true;
            }

            if (MembershipDetails.SlackMemberId != membership.SlackMemberId)
            {
                membership.SlackMemberId = MembershipDetails.SlackMemberId;
                membershipChanged = true;
            }

            if (membershipChanged)
            {
                await _membershipStore.UpdateMembershipAsync(membership);
                StatusMessage = "Your membership has been updated.";
            }
            else
            {
                StatusMessage = "No changes detected.";
            }

            return RedirectToPage();
        }
         
        private async Task<HeadLightMembership> Load(ClaimsPrincipal user)
        {
            long userId = long.Parse(_userManager.GetUserId(User));
            IList<HeadLightMembership> memberships = await _membershipStore.RetrieveMembershipsByUserIdAsync(userId);
            return memberships.First();
        }

        private readonly HeadLightMembershipStore _membershipStore;
        private readonly UserManager<HeadLightUser> _userManager;
        private readonly ILogger<MembershipModel> _logger;
    }
}