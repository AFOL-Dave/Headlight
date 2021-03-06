using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data.Entity;

namespace Headlight.Data
{
    public interface IUserDataClient
    {
        Task CreateLoginAsync(ILoginEntity loginEntity, CancellationToken cancellationToken = new());

        Task CreateUserAsync(IUserEntity userEntity, CancellationToken cancellationToken = new());

        Task DeleteLoginAsync(ILoginEntity loginEntity, CancellationToken cancellationToken = new());

        Task DeleteUserAsync(IUserEntity userEntity, CancellationToken cancellationToken = new());

        Task<IList<ILoginEntity>> RetrieveLoginsByUserIdAsync(long userId, CancellationToken cancellationToken = new());

        Task<IUserEntity> RetrieveUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new());

        Task<IUserEntity> RetrieveUserByLoginAsync(string providerId, string providerKey, CancellationToken cancellationToken = new());

        Task<IUserEntity> RetrieveUserByUserIdAsync(long userId, CancellationToken cancellationToken = new());

        Task<IUserEntity> RetrieveUserByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = new());

        Task UpdateUserAsync(IUserEntity userEntity, CancellationToken cancellationToken = new());
    }
}