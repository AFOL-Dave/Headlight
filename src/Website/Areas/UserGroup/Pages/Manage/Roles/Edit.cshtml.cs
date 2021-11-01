using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Headlight.Areas.UserGroup.Models;
using Headlight.Models;
using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Headlight.Areas.UserGroup.Pages.Manage.Roles
{
    public class RoleEditModel : PageModel
    {
        public IList<HeadLightRoleMembership> AssignedMembers {get; set;}

        public IList<HeadLightMembership> AvailableMembers {get; set;}

        public IList<HeadLightRoleRight> Rights {get; set;}

        public RoleDetails Role {get; set;}

        [TempData]
        public string StatusMessage { get; set; }

        public RoleEditModel( HeadLightRoleStore roleStore,
                              HeadLightMembershipStore membershipStore,
                              ILogger<RoleEditModel> logger )
        {
            _roleStore = roleStore;
            _membershipStore = membershipStore;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(long roleId)
        {
            await Load(roleId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            long roleId = long.Parse(Request.Form["RoleId"].ToString());

            IList<HeadLightRoleRight> existingRights = await _roleStore.RetrieveRightsByRoleIdAsync(roleId);

            foreach (HeadLightRoleRight right in existingRights)
            {
                RightState submittedValue = Request.Form[string.Concat("Right_", right.Id)].ToRightState();

                if (right.State != submittedValue)
                {
                    right.State = submittedValue;
                    await _roleStore.UpdateRoleRightAsync(right);
                }
            }

            IList<HeadLightRoleMembership> existingMemebrships = await _roleStore.RetrieveRoleMembershipByRoleIdAsync(roleId);

            string[] assigned = Request.Form["from"].ToArray();

            foreach(string assignedMember in assigned)
            {
                if (existingMemebrships.All(m => m.MembershipId.ToString() != assignedMember))
                {
                    HeadLightRoleMembership newRoleMembership = new HeadLightRoleMembership
                    {
                        RoleId = roleId,
                        MembershipId = long.Parse(assignedMember)
                    };
                    await _roleStore.CreateRoleMembershipAsync(newRoleMembership);
                }
            }

            foreach(HeadLightRoleMembership existingMember in existingMemebrships)
            {
                if (assigned.All(s => s != existingMember.Id.ToString()))
                {
                    await _roleStore.DeleteRoleMembershipAsync(existingMember);
                }
            }

            return RedirectToPage(new { roleId = roleId });
        }

        private async Task Load(long roleId)
        {
            RoleDetails details = new RoleDetails();

            HeadLightRole role = await _roleStore.FindByIdAsync(roleId.ToString());
            details.Id = roleId;
            details.Name = role.Name;
            
            AssignedMembers = await _roleStore.RetrieveRoleMembershipByRoleIdAsync(roleId);

            AvailableMembers = await _membershipStore.RetrieveMembershipsByUserGroupIdAsync(role.UserGroupId);

            foreach(HeadLightRoleMembership roleMembership in AssignedMembers)
            {
                HeadLightMembership alreadyAssignedMembership = AvailableMembers.FirstOrDefault(m => m.Id == roleMembership.MembershipId);
                AvailableMembers.Remove(alreadyAssignedMembership);
            }

            Role = details;

            Rights = await _roleStore.RetrieveRightsByRoleIdAsync(roleId);
        }

        private readonly HeadLightRoleStore _roleStore;
        private readonly HeadLightMembershipStore _membershipStore;
        private readonly ILogger<RoleEditModel> _logger;
    }
}