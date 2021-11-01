using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data;
using Headlight.Data.Entity;
using Microsoft.Extensions.Logging;

namespace Headlight.Models
{
    public class HeadLightMembershipStore
    {
        public HeadLightMembershipStore(IMembershipDataClient membershipDataClient, ILogger<HeadLightMembershipStore> logger)
        {
            this.membershipDataClient = membershipDataClient;
            this.logger = logger;
        }

        public async Task CreateMembershipAsync(HeadLightMembership membership, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateMembershipAsync");

            IMembershipEntity membershipEntity = LoadEntity(membership);

            try
            {
                await membershipDataClient.CreateMembershipAsync(membershipEntity, cancellationToken);
                membership.Id = membershipEntity.Id;

                logger.LogInformation("Successfully Leaving CreateMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in CreateMembershipAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task DeleteMembershipAsync(HeadLightMembership membership, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteMembershipAsync");

            IMembershipEntity membershipEntity = LoadEntity(membership);

            try
            {
                await membershipDataClient.DeleteMembershipAsync(membershipEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving DeleteMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in DeleteMembershipAsync {e.HResult}, {e.Message}");
            }
        }

        public async Task<HeadLightMembership> RetrieveMembershipByMembershipIdAsync(long membershipId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipByMembershipIdAsync");

            try
            {
                IMembershipEntity membershipEntity = await membershipDataClient.RetrieveMembershipByMembershipIdAsync(membershipId, cancellationToken);
                HeadLightMembership membership = LoadModel(membershipEntity);

                logger.LogInformation("Successfully Leaving RetrieveMembershipByMembershipIdAsync");

                return membership;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveMembershipByMembershipIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<IList<HeadLightMembership>> RetrieveMembershipsByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipsByUserGroupIdAsync");

            try
            {
                IList<IMembershipEntity> membershipEntities = await membershipDataClient.RetrieveMembershipsByUserGroupIdAsync(userGroupId, cancellationToken);
                IList<HeadLightMembership> memberships = LoadModels(membershipEntities);

                logger.LogInformation("Successfully Leaving RetrieveMembershipsByUserGroupIdAsync");

                return memberships;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveMembershipsByUserGroupIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task<IList<HeadLightMembership>> RetrieveMembershipsByUserIdAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipsByUserIdAsync");

            try
            {
                IList<IMembershipEntity> membershipEntities = await membershipDataClient.RetrieveMembershipsByUserIdAsync(userId, cancellationToken);
                IList<HeadLightMembership> memberships = LoadModels(membershipEntities);

                logger.LogInformation("Successfully Leaving RetrieveMembershipsByUserIdAsync");

                return memberships;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveMembershipsByUserIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public async Task UpdateMembershipAsync(HeadLightMembership membership, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateMembershipAsync");

            IMembershipEntity membershipEntity = LoadEntity(membership);

            try
            {
                await membershipDataClient.UpdateMembershipAsync(membershipEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving UpdateMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in UpdateMembershipAsync {e.HResult}, {e.Message}");
            }
        }

        private IMembershipEntity LoadEntity(HeadLightMembership membership)
        {
            return new MembershipEntity
            {
                Id = membership.Id,
                IsActive = membership.IsActive,
                IsCurrent = membership.IsCurrent,
                IsPrimary = membership.IsPrimary,
                UserGroupId = membership.UserGroupId,
                SlackMemberId = membership.SlackMemberId,
                UserId = membership.UserId
            };
        }

        private HeadLightMembership LoadModel(IMembershipEntity entity)
        {
            HeadLightMembership result = new HeadLightMembership
                {
                    Id = entity.Id,
                    IsActive = entity.IsActive,
                    IsCurrent = entity.IsCurrent,
                    IsPrimary = entity.IsPrimary,
                    UserGroupId = entity.UserGroupId,
                    SlackMemberId = entity.SlackMemberId,
                    UserId = entity.UserId,
                    DisplayName = entity.DisplayName,
                    GivenName = entity.GivenName,
                    SurName = entity.SurName
                };

            return result;
        }

        private IList<HeadLightMembership> LoadModels(IList<IMembershipEntity> membershipEntities)
        {
            IList<HeadLightMembership> result = new List<HeadLightMembership>();

            foreach (IMembershipEntity entity in membershipEntities)
            {
                result.Add(new HeadLightMembership
                {
                    Id = entity.Id,
                    IsActive = entity.IsActive,
                    IsCurrent = entity.IsCurrent,
                    IsPrimary = entity.IsPrimary,
                    UserGroupId = entity.UserGroupId,
                    SlackMemberId = entity.SlackMemberId,
                    UserId = entity.UserId,
                    DisplayName = entity.DisplayName,
                    GivenName = entity.GivenName,
                    SurName = entity.SurName
                });
            }

            return result;
        }

        private IMembershipDataClient membershipDataClient;
        private ILogger<HeadLightMembershipStore> logger;
    }
}