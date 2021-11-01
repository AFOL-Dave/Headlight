using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data.Entity;

namespace Headlight.Data
{
    public interface IRoleDataClient
    {
        Task CreateMembershipRoleAsync(IMembershipRoleEntity membershipRoleEntity, CancellationToken cancellationToken = new());

        Task CreateRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken = new ());

        Task DeleteMembershipRoleAsync(IMembershipRoleEntity membershipRoleEntity, CancellationToken cancellationToken = new ());

        Task DeleteRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken = new ());

        Task<IList<IMembershipRoleEntity>> RetrieveMembershipRolesByRoleId(long roleId, CancellationToken cancellationToken = new ());

        Task<IList<IRightEntity>> RetrieveRightsByRoleIdAsync(long roleId, CancellationToken cancellationToken = new ());

        Task<IRoleEntity> RetrieveRoleByRoleIdAsync(long roleId, CancellationToken cancellationToken = new ());

        Task<IList<IRoleEntity>> RetrieveRolesByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ());

        Task UpdateRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken = new ());

        Task UpdateRoleRightAsync(IRightEntity rightEntity, CancellationToken cancellationToken = new ());
    }
}