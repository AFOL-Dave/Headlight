using Headlight.Models.Enumerations;

namespace Headlight.Models
{
    public class HeadLightUserGroup
    {
        public long Id { get; set; }

        public string DisplayName
        {
            get
            {
                return string.IsNullOrWhiteSpace(ShortName) ? FullName : FullName + " | " + ShortName;
            }
        }

        public string FullName { get; set; }

        public UserGroupJoinType JoinType { get; set; }

        public string NormalizedFullName { get; set; }

        public string NormalizedShortName { get; set; }

        public string ShortName { get; set; }

        public string SlackWorkspaceId { get; set; }
    }
}