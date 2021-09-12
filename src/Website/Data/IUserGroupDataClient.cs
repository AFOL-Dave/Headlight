using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data.Entity;

namespace Headlight.Data
{
    public interface IUserGroupDataClient
    {
        Task CreateUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = new());

        Task DeleteUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = new());

        Task<IUserGroupEntity> RetrieveUserGroupAsync(CancellationToken cancellationToken = new());

        Task<IUserGroupEntity> RetrieveUserGroupByFullNameAsync(string normalizedFullName, CancellationToken cancellationToken = new());

        Task<IUserGroupEntity> RetrieveUserGroupByShortNameAsync(string normalizedShortName, CancellationToken cancellationToken = new());

        Task<IUserGroupEntity> RetrieveUserGroupByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new());

        Task<IList<IUserGroupEntity>> RetrieveUserGroupsByJoinTypeAsync(byte joinTypes, CancellationToken cancellationToken = new());

        Task UpdateUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = new());
    }
}