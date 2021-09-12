using System.Security.Claims;
using System.Threading.Tasks;
using Headlight.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Areas.UserGroup.Pages.Manage
{
    public class UserGroupModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public HeadLightUserGroup UserGroup { get; set; }

        public UserGroupModel( HeadLightUserGroupStore userGroupStore,
                               UserManager<HeadLightUser> userManager )
        {
            _userGroupStore = userGroupStore;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserGroup = await Load(User);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                UserGroup = await Load(User);
                return Page();
            }

            HeadLightUserGroup userGroup = await Load(User);
            bool userGroupChanged = false;

            if (userGroup.FullName != UserGroup.FullName)
            {
                userGroup.FullName = UserGroup.FullName;
                userGroupChanged = true;
            }

            if (userGroup.ShortName != UserGroup.ShortName)
            {
                userGroup.ShortName = UserGroup.ShortName;
                userGroupChanged = true;
            }

            if (userGroup.SlackWorkspaceId != UserGroup.SlackWorkspaceId)
            {
                userGroup.SlackWorkspaceId = UserGroup.SlackWorkspaceId;
                userGroupChanged = true;
            }

            if (userGroup.JoinType != UserGroup.JoinType)
            {
                userGroup.JoinType = UserGroup.JoinType;
                userGroupChanged = true;
            }

            if (userGroupChanged)
            {
                await _userGroupStore.UpdateUserGroupAsync(userGroup);
                StatusMessage = "The User Group was saved successfully.";
            }
            else
            {
                StatusMessage = "The User Group was not changed.";
            }

            return RedirectToPage();
        }

        private async Task<HeadLightUserGroup> Load(ClaimsPrincipal principal)
        {
            long userId = long.Parse(_userManager.GetUserId(principal));
            HeadLightUserGroup userGroup = await _userGroupStore.RetrieveCurrentUserGroupAsync(userId);
            return userGroup;
        }

        private readonly HeadLightUserGroupStore _userGroupStore;
        private readonly UserManager<HeadLightUser> _userManager;
    }
}