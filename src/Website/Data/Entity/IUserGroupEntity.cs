namespace Headlight.Data.Entity
{
    public interface IUserGroupEntity
    {
        string FullName { get; set; }

        long Id { get; set; }

        byte JoinType { get; set; }

        string NormalizedFullName { get; set; }

        string NormalizedShortName { get; set; }

        string ShortName { get; set; }

        string SlackWorkspaceId { get; set; }
    }
}