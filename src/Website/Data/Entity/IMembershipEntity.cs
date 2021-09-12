namespace Headlight.Data.Entity
{
    public interface IMembershipEntity
    {
        long Id { get; set; }

        bool IsActive { get; set; }

        bool IsCurrent { get; set; }

        bool IsPrimary { get; set; }

        string SlackMemberId { get; set; }

        long UserGroupId { get; set; }

        long UserId { get; set; }
    }
}