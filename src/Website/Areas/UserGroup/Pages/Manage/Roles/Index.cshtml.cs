using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Headlight.Areas.UserGroup.Models;
using Headlight.Models;
using Headlight.Models.Attributes;
using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.UserGroup.Pages.Manage.Roles
{
    [Authorize(Policy = "MaintainRoles")]
    public class RoleIndexModel : PageModel
    {
        public List<RoleDetails> Roles { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public long UserGroupId { get; set; }

        public RoleIndexModel( HeadLightRoleStore roleStore,
                               HeadLightMembershipStore membershipStore,
                               IAuthorizationService authorizationService,
                               UserManager<HeadLightUser> userManager,
                               ILogger<RoleIndexModel> logger )
        {
            _membershipStore = membershipStore;
            _roleStore = roleStore;
            _authorizationService = authorizationService;
            _usermanager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await Load(User);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRoleAsync(long roleId)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, Right.DeleteRole.GetPolicyName())).Succeeded)
            {
                StatusMessage = "ERROR: You are not authorized to delete roles.";
                return RedirectToPage();
            }

            HeadLightRole role = new HeadLightRole
            {
                Id = roleId
            };

            await _roleStore.DeleteAsync(role);

            return RedirectToPage();
        }

        private async Task<List<RoleDetails>> Load(ClaimsPrincipal principal)
        {
            long userId = long.Parse(_usermanager.GetUserId(principal));
            HeadLightMembership membership = (await _membershipStore.RetrieveMembershipsByUserIdAsync(userId)).First(m => m.IsCurrent);
            UserGroupId = membership.UserGroupId;

            List<HeadLightRole> roles = (await _roleStore.RetrieveRolesByUserGroupIdAsync(UserGroupId)).ToList();
            List<RoleDetails> details = new List<RoleDetails>();

            foreach(HeadLightRole role in roles)
            {
                details.Add(new RoleDetails
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return details;
        }

        private readonly HeadLightRoleStore _roleStore;
        private readonly HeadLightMembershipStore _membershipStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<HeadLightUser> _usermanager;
        private readonly ILogger<RoleIndexModel> _logger;
    }
}