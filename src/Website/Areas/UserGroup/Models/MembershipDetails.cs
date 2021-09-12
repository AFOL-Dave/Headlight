namespace Headlight.Areas.UserGroup.Models
{
    public class MembershipDetails
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrimary { get; set; }

        public string SlackMemberId { get; set; }
    }
}