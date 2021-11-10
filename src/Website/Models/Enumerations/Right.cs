using Headlight.Models.Attributes;

namespace Headlight.Models.Enumerations
{
    public enum Right
    {
        [PolicyName("CanCreateRole")]
        CreateRole = 1,

        [PolicyName("CanUpdateRole")]
        UpdateRole = 2,

        [PolicyName("CanDeleteRole")]
        DeleteRole = 3,

        [PolicyName("CanMaintainUserGroupProfile")]
        MaintainUserGroupProfile = 4,

        [PolicyName("CanMaintainMemberships")]
        MaintainMemberships = 5
    }
}