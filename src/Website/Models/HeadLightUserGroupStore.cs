using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data;
using Headlight.Data.Entity;
using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Headlight.Models
{
    public class HeadLightUserGroupStore
    {
        public HeadLightUserGroupStore(IUserGroupDataClient userGroupDataClient, IMembershipDataClient membershipDataClient, ILookupNormalizer normalizer, ILogger<HeadLightUserGroupStore> logger)
        {
            this.userGroupDataClient = userGroupDataClient;
            this.membershipDataClient = membershipDataClient;
            this.logger = logger;
            this.normalizer = normalizer;
        }

        public async Task<IdentityResult> CreateUserGroupAsync(HeadLightUserGroup userGroup, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateUserGroupAsync");

            IUserGroupEntity userGroupEntity = LoadEntity(userGroup);

            try
            {
                await userGroupDataClient.CreateUserGroupAsync(userGroupEntity, cancellationToken);

                userGroup.Id = userGroupEntity.Id;
                userGroup.NormalizedFullName = userGroupEntity.NormalizedFullName;
                userGroup.NormalizedShortName = userGroupEntity.NormalizedShortName;

                logger.LogInformation("Successfully Leaving CreateUserGroupAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in CreateUserGroupAsync {e.HResult}, {e.Message}");

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

        public async Task<IdentityResult> DeleteUserGroupAsync(HeadLightUserGroup userGroup, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteUserGroupAsync");

            IUserGroupEntity userGroupEntity = LoadEntity(userGroup);

            try
            {
                await userGroupDataClient.DeleteUserGroupAsync(userGroupEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving DeleteUserGroupAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in DeleteUserGroupAsync {e.HResult}, {e.Message}");

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

        public async Task<HeadLightUserGroup> RetrieveCurrentUserGroupAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveCurrentUserGroupAsync");

            try
            {
                IUserGroupEntity userGroupEntity = await membershipDataClient.RetrieveCurrentUserGroupAsync(userId, cancellationToken);

                HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                
                logger.LogInformation("Successfully Leaving RetrieveCurrentUserGroupAsync");

                return userGroup;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveCurrentUserGroupAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        /// <summary>
        ///  Retrieves the one, and only one, user group.
        /// </summary>
        /// <remarks>
        ///  This method should only be called if the web application is in single-LUG mode.
        /// </remarks>
        public async Task<HeadLightUserGroup> RetrieveUserGroupAsync(CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupAsync");

            try
            {
                IUserGroupEntity userGroupEntity = await userGroupDataClient.RetrieveUserGroupAsync(cancellationToken);

                HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                
                logger.LogInformation("Successfully Leaving RetrieveUserGroupAsync");

                return userGroup;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveUserGroupAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<HeadLightUserGroup> RetrieveUserGroupByFullNameAsync(string fullName, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupByFullNameAsync");

            fullName = normalizer.NormalizeName(fullName);

            try
            {
                IUserGroupEntity userGroupEntity = await userGroupDataClient.RetrieveUserGroupByFullNameAsync(fullName, cancellationToken);

                HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                
                logger.LogInformation("Successfully Leaving RetrieveUserGroupByFullNameAsync");

                return userGroup;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveUserGroupByFullNameAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<HeadLightUserGroup> RetrieveUserGroupByShortNameAsync(string shortName, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupByShortNameAsync");

            shortName = normalizer.NormalizeName(shortName);

            try
            {
                IUserGroupEntity userGroupEntity = await userGroupDataClient.RetrieveUserGroupByShortNameAsync(shortName, cancellationToken);

                HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                
                logger.LogInformation("Successfully Leaving RetrieveUserGroupByShortNameAsync");

                return userGroup;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveUserGroupByShortNameAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<IList<HeadLightUserGroup>> RetrieveUserGroupsByUserIdAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupsByUserIdAsync");

            try
            {
                IList<IUserGroupEntity> userGroupEntities = await membershipDataClient.RetrieveUserGroupsByUserIdAsync(userId, cancellationToken);
                IList<HeadLightUserGroup> userGroups = new List<HeadLightUserGroup>();

                foreach (IUserGroupEntity userGroupEntity in userGroupEntities)
                {
                    HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                    userGroups.Add(userGroup);
                }

                logger.LogInformation("Successfully Leaving RetrieveUserGroupsByUserIdAsync");

                return userGroups;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveUserGroupsByUserIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<IList<HeadLightUserGroup>> RetrieveUserGroupsByJoinTypeAsync(UserGroupJoinType joinType, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupsByJoinTypeAsync");

            try
            {
                IList<IUserGroupEntity> userGroupEntities = await userGroupDataClient.RetrieveUserGroupsByJoinTypeAsync((byte)joinType, cancellationToken);
                IList<HeadLightUserGroup> userGroups = new List<HeadLightUserGroup>();

                foreach (IUserGroupEntity userGroupEntity in userGroupEntities)
                {
                    HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                    userGroups.Add(userGroup);
                }

                logger.LogInformation("Successfully Leaving RetrieveUserGroupByShortNameAsync");

                return userGroups;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveUserGroupByShortNameAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<HeadLightUserGroup> RetrieveUserGroupByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupByUserGroupIdAsync");

            try
            {
                IUserGroupEntity userGroupEntity = await userGroupDataClient.RetrieveUserGroupByUserGroupIdAsync(userGroupId, cancellationToken);

                HeadLightUserGroup userGroup = LoadModel(userGroupEntity);
                
                logger.LogInformation("Successfully Leaving DeleteUserGroupAsyncAsync");

                return userGroup;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in DeleteUserGroupAsyncAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserGroupAsync(HeadLightUserGroup userGroup, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered UpdateUserGroupAsync");

            IUserGroupEntity userGroupEntity = LoadEntity(userGroup);

            try
            {
                await userGroupDataClient.UpdateUserGroupAsync(userGroupEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving UpdateUserGroupAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in UpdateUserGroupAsync {e.HResult}, {e.Message}");

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

        private IUserGroupEntity LoadEntity(HeadLightUserGroup userGroup)
        {
            if (userGroup != null)
            {
                return new UserGroupEntity
                {
                    Id = userGroup.Id,
                    FullName = userGroup.FullName,
                    JoinType = (byte) userGroup.JoinType,
                    NormalizedFullName = normalizer.NormalizeName(userGroup.FullName),
                    NormalizedShortName = normalizer.NormalizeName(userGroup.ShortName),
                    ShortName = userGroup.ShortName,
                    SlackWorkspaceId = userGroup.SlackWorkspaceId
                };
            }

            return null;
        }

        private HeadLightUserGroup LoadModel(IUserGroupEntity userGroupEntity)
        {
            if (userGroupEntity != null)
            {
                return new HeadLightUserGroup
                {
                    Id = userGroupEntity.Id,
                    FullName = userGroupEntity.FullName,
                    JoinType = (UserGroupJoinType)userGroupEntity.JoinType,
                    NormalizedFullName = userGroupEntity.NormalizedFullName,
                    NormalizedShortName = userGroupEntity.NormalizedShortName,
                    ShortName = userGroupEntity.ShortName,
                    SlackWorkspaceId = userGroupEntity.SlackWorkspaceId
                };
            }

            return null;
        }

        private IMembershipDataClient membershipDataClient;
        private readonly IUserGroupDataClient userGroupDataClient;
        private readonly ILogger<HeadLightUserGroupStore> logger;
        private readonly ILookupNormalizer normalizer;
    }
}