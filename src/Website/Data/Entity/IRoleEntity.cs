namespace Headlight.Data.Entity
{
    public interface IRoleEntity
    {
        long Id { get; set; }

        bool IsDefault { get; set; }

        string Name { get; set; }

        string NormalizedName { get; set; }

        long UserGroupId { get; set; }
    }
}