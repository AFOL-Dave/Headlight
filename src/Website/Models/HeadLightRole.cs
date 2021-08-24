using Microsoft.AspNetCore.Identity;

namespace Headlight.Models
{
    public class HeadLightRole : IdentityRole<long>
    {
        public long UserGroupId { get; set; }

        public bool IsDefault { get; set; }
    }
}