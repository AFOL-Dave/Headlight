using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Headlight.Areas.UserGroup.Models;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Areas.UserGroup.Pages.Manage
{
    public class MembersModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public List<MembershipDetails> MembershipDetails { get; set; }

        [TempData]
        public string StatusMessage {get; set;}

        public MembersModel( HeadLightMembershipStore membershipStore,
                             UserManager<HeadLightUser> userManager )
        {
            _membershipStore = membershipStore;
            _usermanager = userManager;

            StatusMessage = "";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            MembershipDetails = await Load(User);

            return Page();
        }

        public async Task<IActionResult> OnPostExpelAsync(long membershipId)
        {
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleActivationAsync(long membershipId)
        {
            HeadLightMembership membership = await _membershipStore.RetrieveMembershipByMembershipIdAsync(membershipId);

            membership.IsActive = !membership.IsActive;

            await _membershipStore.UpdateMembershipAsync(membership);

            MembershipDetails = await Load(User);

            return RedirectToPage();
        }

        private async Task<List<MembershipDetails>> Load(ClaimsPrincipal principal)
        {
            long userId = long.Parse(_usermanager.GetUserId(principal));
            HeadLightMembership membership = (await _membershipStore.RetrieveMembershipsByUserIdAsync(userId)).First(m => m.IsCurrent);
            List<HeadLightMembership> members = new List<HeadLightMembership>(await _membershipStore.RetrieveMembershipsByUserGroupIdAsync(membership.UserGroupId));
            List<MembershipDetails> details = new List<MembershipDetails>();

            foreach(HeadLightMembership member in members)
            {
                HeadLightUser user = await _usermanager.FindByIdAsync(member.UserId.ToString());
                details.Add(new MembershipDetails
                {
                    Id = member.Id,
                    FullName = user.GivenName + " " + user.SurName + " (" + user.DisplayName + ")",
                    IsActive = member.IsActive,
                    IsPrimary = member.IsPrimary
                });
            }

            return details;
        }

        private HeadLightMembershipStore _membershipStore;
        private UserManager<HeadLightUser> _usermanager;
    }
}