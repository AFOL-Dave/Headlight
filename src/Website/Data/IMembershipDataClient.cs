using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data.Entity;

namespace Headlight.Data
{
    public interface IMembershipDataClient
    {
        Task CreateMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ());

        Task DeleteMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ());

        Task<IUserGroupEntity> RetrieveCurrentUserGroupAsync(long userId, CancellationToken cancellationToken = new ());

        Task<IMembershipEntity> RetrieveMembershipByMembershipIdAsync(long membershipId, CancellationToken cancellationToken = new ());

        Task<IList<IUserGroupEntity>> RetrieveUserGroupsByUserIdAsync(long userId, CancellationToken cancellationToken = new ());

        Task<IList<IMembershipEntity>> RetrieveMembershipsByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ());

        Task<IList<IMembershipEntity>> RetrieveMembershipsByUserIdAsync(long userId, CancellationToken cancellationToken = new ());

        Task UpdateMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ());
    }
}