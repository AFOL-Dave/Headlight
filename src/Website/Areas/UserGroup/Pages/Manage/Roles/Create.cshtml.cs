using System.Threading.Tasks;
using Headlight.Areas.UserGroup.Models;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.UserGroup.Pages.Manage.Roles
{
    public class RoleCreateModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public RoleDetails RoleDetails { get; set; }

        [BindProperty]
        public long UserGroupId { get; set; }

        public RoleCreateModel( HeadLightRoleStore roleStore,
                                ILogger<RoleCreateModel> logger )
        {
            _roleStore = roleStore;
            _logger = logger;
        }

        public void OnGet(long userGroupId)
        {
            UserGroupId = userGroupId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HeadLightRole newRole = new HeadLightRole
            {
                Name = RoleDetails.Name,
                NormalizedName = RoleDetails.Name,
                UserGroupId = UserGroupId
            };

            await _roleStore.CreateAsync(newRole);
            return RedirectToPage();
        }

        private readonly HeadLightRoleStore _roleStore;
        private readonly ILogger<RoleCreateModel> _logger;
    }
}