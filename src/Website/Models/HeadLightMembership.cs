namespace Headlight.Models
{
    public class HeadLightMembership
    {
        public long Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsPrimary { get; set; }

        public string SlackMemberId { get; set; }

        public long UserId { get; set; }

        public long UserGroupId { get; set; }
    }
}