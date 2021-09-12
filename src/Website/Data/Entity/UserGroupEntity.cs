namespace Headlight.Data.Entity
{
    public class UserGroupEntity : IUserGroupEntity
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public byte JoinType { get; set; }

        public string NormalizedFullName { get; set; }

        public string NormalizedShortName { get; set; }

        public string ShortName { get; set; }

        public string SlackWorkspaceId { get; set; }
    }
}