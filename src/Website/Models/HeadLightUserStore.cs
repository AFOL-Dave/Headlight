using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data;
using Headlight.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Headlight.Models
{
    public class HeadLightUserStore: IUserPasswordStore<HeadLightUser>, IUserEmailStore<HeadLightUser>, IUserClaimStore<HeadLightUser>, IUserLoginStore<HeadLightUser>, IUserPhoneNumberStore<HeadLightUser>
    {
        public HeadLightUserStore(IUserDataClient userDataClient, ILogger<HeadLightUserStore> logger)
        {
            this.userDataClient = userDataClient;
            this.logger = logger;
        }

        public Task AddClaimsAsync(HeadLightUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered AddClaimsAsync");

            foreach (Claim c in claims)
            {
                user.Claims.Add(c);
            }

            logger.LogInformation("Successfully Leaving AddClaimsAsync");
            return Task.CompletedTask;
        }

        public async Task AddLoginAsync(HeadLightUser user, UserLoginInfo login, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered AddLoginAsync");

            ILoginEntity loginEntity = new LoginEntity
            {
                ProviderId = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName,
                UserId = user.Id
            };

            await userDataClient.CreateLoginAsync(loginEntity, cancellationToken);

            logger.LogInformation("Successfully Leaving AddLoginAsync");
        }

        public async Task<IdentityResult> CreateAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateAsync");

            IUserEntity entity = LoadEntity(user);

            try
            {
                await userDataClient.CreateUserAsync(entity, cancellationToken);
                user.Id = entity.Id;
                logger.LogInformation("Successfully Leaving CreateAsync");
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in CreateAsync {e.HResult}, {e.Message}");

                IdentityError[] errs =
                {
                    new ()
                    {
                        Code = e.HResult.ToString(),
                        Description = e.Message
                    }
                };

                return IdentityResult.Failed(errs);
            }
        }

        public async Task<IdentityResult> DeleteAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteAsync");

            try
            {
                await userDataClient.DeleteUserAsync(LoadEntity(user), cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteAsync");
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in DeleteAsync {e.HResult}, {e.Message}");

                IdentityError[] errs =
                {
                    new ()
                    {
                        Code = e.HResult.ToString(),
                        Description = e.Message
                    }
                };

                return IdentityResult.Failed(errs);
            }
        }

        public void Dispose()
        {
            logger.LogInformation("Entered Dispose");
            logger.LogInformation("Successfully Leaving Dispose");
        }

        public async Task<HeadLightUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered FindByEmailAsync");

            IUserEntity entity = await userDataClient.RetrieveUserByEmailAsync(normalizedEmail, cancellationToken);

            return LoadModel(entity);
        }

        public async Task<HeadLightUser> FindByIdAsync(string userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered FindByIdAsync");

            IUserEntity entity = await userDataClient.RetrieveUserByUserIdAsync(long.Parse(userId), cancellationToken);

            return LoadModel(entity);
        }

        public async Task<HeadLightUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered FindByLoginAsync");

            IUserEntity entity = await userDataClient.RetrieveUserByLoginAsync(loginProvider, providerKey, cancellationToken);

            return LoadModel(entity);
        }

        public async Task<HeadLightUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered FindByNameAsync");

            IUserEntity entity = await userDataClient.RetrieveUserByUserNameAsync(normalizedUserName, cancellationToken);

            return LoadModel(entity);
        }

        public Task<IList<Claim>> GetClaimsAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetClaimsAsync");

            logger.LogInformation("Successfully Leaving GetClaimsAsync");
            return Task.FromResult(user.Claims);
        }

        public Task<string> GetEmailAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetEmailAsync");

            logger.LogInformation("Successfully Leaving GetEmailAsync");
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetEmailConfirmedAsync");

            logger.LogInformation("Successfully Leaving GetEmailConfirmedAsync");
            return Task.FromResult(true);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetLoginsAsync");

            IList<ILoginEntity> loginEntities = await userDataClient.RetrieveLoginsByUserIdAsync(user.Id, cancellationToken);

            if (loginEntities.Any())
            {
                List<UserLoginInfo> logins = new List<UserLoginInfo>();

                foreach (ILoginEntity loginEntity in loginEntities)
                {
                    logins.Add(new(loginEntity.ProviderId, loginEntity.ProviderKey, loginEntity.ProviderDisplayName));
                }

                logger.LogInformation("Successfully Leaving GetLoginsAsync");
                return logins;
            }

            logger.LogInformation("Successfully Leaving GetLoginsAsync");
            return null;
        }

        public Task<string> GetNormalizedEmailAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetNormalizedEmailAsync");

            logger.LogInformation("Successfully Leaving GetNormalizedEmailAsync");
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetNormalizedUserNameAsync");

            logger.LogInformation("Successfully Leaving GetNormalizedUserNameAsync");
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPhoneNumberAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetPhoneNumberAsync");

            logger.LogInformation("Successfully Leaving GetPhoneNumberAsync");
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetPhoneNumberConfirmedAsync");

            logger.LogInformation("Successfully Leaving GetPhoneNumberConfirmedAsync");
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<string> GetPasswordHashAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetPasswordHashAsync");

            logger.LogInformation("Successfully Leaving GetPasswordHashAsync");
            return Task.FromResult(string.Empty);
        }

        public Task<string> GetUserIdAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetUserIdAsync");

            logger.LogInformation("Successfully Leaving GetUserIdAsync");
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetUserNameAsync");

            logger.LogInformation("Successfully Leaving GetUserNameAsync");
            return Task.FromResult(user.UserName);
        }

        public Task<IList<HeadLightUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered GetUsersForClaimAsync");

            logger.LogInformation("Successfully Leaving GetUsersForClaimAsync");
            return null;
        }

        public Task<bool> HasPasswordAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered HasPasswordAsync");

            logger.LogInformation("Successfully Leaving HasPasswordAsync");
            return Task.FromResult(false);
        }

        public Task RemoveClaimsAsync(HeadLightUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RemoveClaimsAsync");

            foreach (Claim claim in claims)
            {
                user.Claims.Remove(claim);
            }

            logger.LogInformation("Successfully Leaving RemoveClaimsAsync");
            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(HeadLightUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RemoveLoginAsync");

            ILoginEntity loginEntity = new LoginEntity
            {
                ProviderId = loginProvider,
                ProviderKey = providerKey,
                UserId = user.Id
            };

            return userDataClient.DeleteLoginAsync(loginEntity, cancellationToken);
        }

        public Task ReplaceClaimAsync(HeadLightUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered ReplaceClaimAsync");

            user.Claims.Remove(claim);
            user.Claims.Add(newClaim);

            logger.LogInformation("Successfully Leaving ReplaceClaimAsync");
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(HeadLightUser user, string email, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetEmailAsync");

            user.Email = email;

            logger.LogInformation("Successfully Leaving SetEmailAsync");
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(HeadLightUser user, bool confirmed, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetEmailConfirmedAsync");

            user.EmailConfirmed = true;

            logger.LogInformation("Successfully Leaving SetEmailConfirmedAsync");
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(HeadLightUser user, string normalizedEmail, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetNormalizedEmailAsync");

            user.NormalizedEmail = normalizedEmail;

            logger.LogInformation("Successfully Leaving SetNormalizedEmailAsync");
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(HeadLightUser user, string normalizedName, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetNormalizedUserNameAsync");

            user.NormalizedUserName = normalizedName;

            logger.LogInformation("Successfully Leaving SetNormalizedUserNameAsync");
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(HeadLightUser user, string passwordHash, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetPasswordHashAsync");

            logger.LogInformation("Successfully Leaving SetPasswordHashAsync");
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(HeadLightUser user, string phoneNumber, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetPhoneNumberAsync");

            user.PhoneNumber = phoneNumber;

            logger.LogInformation("Successfully Leaving SetPhoneNumberAsync");
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(HeadLightUser user, bool confirmed, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetPhoneNumberConfirmedAsync");

            user.PhoneNumberConfirmed = confirmed;

            logger.LogInformation("Successfully Leaving SetPhoneNumberConfirmedAsync");
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(HeadLightUser user, string userName, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered SetUserNameAsync");

            user.UserName = userName;

            logger.LogInformation("Successfully Leaving SetUserNameAsync");
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(HeadLightUser user, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered UpdateAsync");

            IUserEntity entity = LoadEntity(user);

            try
            {
                await userDataClient.UpdateUserAsync(entity, cancellationToken);
                logger.LogInformation("Successfully Leaving UpdateAsync");
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateAsync");

                IdentityError[] errs =
                {
                    new ()
                    {
                        Code = "1",
                        Description = e.Message
                    }
                };

                return IdentityResult.Failed(errs);
            }
        }

        private IUserEntity LoadEntity(HeadLightUser user)
        {
            if (user != null)
            {
                return new UserEntity
                {
                    Id = user.Id,
                    City = user.City,
                    Country = user.Country,
                    DateOfBirth = user.DateOfBirth,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    GivenName = user.GivenName,
                    IsActive = user.IsActive,
                    IsApproved = user.IsApproved,
                    NormalizedEmail = user.NormalizedEmail,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    PostalCode = user.PostalCode,
                    Region = user.Region,
                    StreetAddressLine1 = user.StreetAddressLine1,
                    StreetAddressLine2 = user.StreetAddressLine2,
                    SurName = user.SurName,
                };
            }

            return null;
        }

        private HeadLightUser LoadModel(IUserEntity entity)
        {
            if (entity != null)
            {
                return new HeadLightUser
                {
                    Id = entity.Id,
                    City = entity.City,
                    Country = entity.Country,
                    DateOfBirth = entity.DateOfBirth,
                    DisplayName = entity.DisplayName,
                    Email = entity.Email,
                    EmailConfirmed = entity.EmailConfirmed,
                    GivenName = entity.GivenName,
                    IsActive = entity.IsActive,
                    IsApproved = entity.IsApproved,
                    NormalizedEmail = entity.NormalizedEmail,
                    NormalizedUserName = entity.DisplayName,
                    PhoneNumber = entity.PhoneNumber,
                    PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                    PostalCode = entity.PostalCode,
                    Region = entity.Region,
                    StreetAddressLine1 = entity.StreetAddressLine1,
                    StreetAddressLine2 = entity.StreetAddressLine2,
                    SurName = entity.SurName,
                    UserName = entity.DisplayName
                };
            }

            return null;
        }

        private readonly IUserDataClient userDataClient;
        private readonly ILogger<HeadLightUserStore> logger;
    }
}