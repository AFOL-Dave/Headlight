using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data;
using Headlight.Data.Entity;
using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Headlight.Models
{
    public class HeadLightRoleStore : IRoleStore<HeadLightRole>
    {
        public HeadLightRoleStore( IRoleDataClient roleDataClient,
                                   ILookupNormalizer normalizer,
                                   ILogger<HeadLightRoleStore> logger)
        {
            this.roleDataClient = roleDataClient;
            this.normalizer = normalizer;
            this.logger = logger;
        }

        public async Task<IdentityResult> CreateAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateAsync");

            IRoleEntity roleEntity = LoadEntity(role);

            try
            {
                await roleDataClient.CreateRoleAsync(roleEntity, cancellationToken);

                role.Id = roleEntity.Id;
                role.Name = roleEntity.Name;
                role.NormalizedName = roleEntity.NormalizedName;
                role.UserGroupId = role.UserGroupId;
                role.IsDefault = role.IsDefault;

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

        public async Task<IdentityResult> CreateRoleMembershipAsync(HeadLightRoleMembership roleMembership, CancellationToken cancellationToken = new ())
        {
            IMembershipRoleEntity membershipRoleEntity = LoadEntity(roleMembership);

            try
            {
                await roleDataClient.CreateMembershipRoleAsync(membershipRoleEntity, cancellationToken);

                roleMembership.Id = membershipRoleEntity.Id;
                roleMembership.MembershipId = membershipRoleEntity.MembershipId;
                roleMembership.RoleId = membershipRoleEntity.RoleId;
                roleMembership.DisplayName = membershipRoleEntity.DisplayName;
                roleMembership.GivenName = membershipRoleEntity.GivenName;
                roleMembership.SurName = membershipRoleEntity.SurName;

                logger.LogInformation("Successfully Leaving CreateRoleMembershipAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in CreateRoleMembershipAsync {e.HResult}, {e.Message}");

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

        public async Task<IdentityResult> DeleteAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteAsync");

            IRoleEntity roleEntity = LoadEntity(role);

            try
            {
                await roleDataClient.DeleteRoleAsync(roleEntity, cancellationToken);

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

        public async Task<IdentityResult> DeleteRoleMembershipAsync(HeadLightRoleMembership roleMembership, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteRoleMembershipAsync");

            IMembershipRoleEntity membershipRoleEntity = LoadEntity(roleMembership);

            try
            {
                await roleDataClient.DeleteMembershipRoleAsync(membershipRoleEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving DeleteRoleMembershipAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in DeleteRoleMembershipAsync {e.HResult}, {e.Message}");

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
        }

        public async Task<HeadLightRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered FindByIdAsync");

            try
            {
                IRoleEntity roleEntity = await roleDataClient.RetrieveRoleByRoleIdAsync(long.Parse(roleId), cancellationToken);
                HeadLightRole role = LoadModel(roleEntity);

                logger.LogInformation("Successfully Leaving FindByIdAsync");

                return role;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in FindByIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public Task<HeadLightRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = new ())
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            return Task.FromResult(role.Name);
        }

        public async Task<IList<HeadLightRoleRight>> RetrieveRightsByRoleIdAsync(long roleId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveRightsByRoleIdAsync");

            try
            {
                IList<IRightEntity> rightEntites = await roleDataClient.RetrieveRightsByRoleIdAsync(roleId, cancellationToken);
                IList<HeadLightRoleRight> rights = LoadModels(rightEntites);

                logger.LogInformation("Successfully leaving RetrieveRightsByRoleIdAsync");

                return rights;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Errir in RetrieveRightsByRoleIdAsync {ex.HResult}, {ex.Message}");

                return null;
            }
        }

        public async Task<IList<HeadLightRoleMembership>> RetrieveRoleMembershipByRoleIdAsync(long roleId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveRoleMembershipByRoleIdAsync");

            try
            {
                IList<IMembershipRoleEntity> membershipRoleEntities = await roleDataClient.RetrieveMembershipRolesByRoleId(roleId, cancellationToken);
                IList<HeadLightRoleMembership> roleMemberships = LoadModels(membershipRoleEntities);

                logger.LogInformation("Successfully leaving RetrieveRoleMembershipByRoleIdAsync");

                return roleMemberships;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Error in RetrieveRoleMembershipByRoleIdAsync {ex.HResult}, {ex.Message}");
                throw;
            }
        }

        public async Task<IList<HeadLightRole>> RetrieveRolesByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveRolesByUserGroupIdAsync");

            try
            {
                IList<IRoleEntity> roleEntity = await roleDataClient.RetrieveRolesByUserGroupIdAsync(userGroupId, cancellationToken);
                IList<HeadLightRole> roles = LoadModels(roleEntity);

                logger.LogInformation("Successfully Leaving RetrieveRolesByUserGroupIdAsync");

                return roles;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in RetrieveRolesByUserGroupIdAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        public Task SetNormalizedRoleNameAsync(HeadLightRole role, string normalizedName, CancellationToken cancellationToken = new ())
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(HeadLightRole role, string roleName, CancellationToken cancellationToken = new ())
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(HeadLightRole role, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered UpdateAsync");

            IRoleEntity roleEntity = LoadEntity(role);

            try
            {
                await roleDataClient.UpdateRoleAsync(roleEntity, cancellationToken);

                logger.LogInformation("Successfully Leaving UpdateAsync");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in UpdateAsync {e.HResult}, {e.Message}");

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

        public async Task UpdateRoleRightAsync(HeadLightRoleRight right, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered UpdateRoleRightAsync");

            IRightEntity rightEntity = LoadEntity(right);

            try
            {
                await roleDataClient.UpdateRoleRightAsync(rightEntity, cancellationToken);

                logger.LogInformation("Successfully leaving UpdateRoleRight");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, $"Error in UpdateRoleRightAsync {e.HResult}, {e.Message}");
                throw;
            }
        }

        private IMembershipRoleEntity LoadEntity(HeadLightRoleMembership roleMembership)
        {
            if (roleMembership != null)
            {
                return new MembershipRoleEntity
                {
                    Id = roleMembership.Id,
                    MembershipId = roleMembership.MembershipId,
                    RoleId = roleMembership.RoleId,
                    DisplayName = roleMembership.DisplayName,
                    GivenName = roleMembership.GivenName,
                    SurName = roleMembership.SurName
                };
            }

            return null;
        }

        private IRightEntity LoadEntity(HeadLightRoleRight right)
        {
            if (right != null)
            {
                return new RightEntity
                {
                    Id = right.Id,
                    Name = right.Name,
                    State = right.State.ToNullableBool()
                };
            }

            return null;
        }

        private IRoleEntity LoadEntity(HeadLightRole role)
        {
            if (role != null)
            {
                return new RoleEntity
                {
                    Id = role.Id,
                    Name = role.Name,
                    NormalizedName = normalizer.NormalizeName(role.NormalizedName),
                    UserGroupId = role.UserGroupId,
                    IsDefault = role.IsDefault
                };
            }

            return null;
        }

        private HeadLightRoleRight LoadModel(IRightEntity rightEntity)
        {
            if (rightEntity != null)
            {
                return new HeadLightRoleRight
                {
                    Id = rightEntity.Id,
                    Name = rightEntity.Name,
                    State = rightEntity.State.ToRightState()
                };
            }

            return null;
        }

        private HeadLightRole LoadModel(IRoleEntity roleEntity)
        {
            if (roleEntity != null)
            {
                return new HeadLightRole
                {
                    Id = roleEntity.Id,
                    Name = roleEntity.Name,
                    NormalizedName = roleEntity.NormalizedName,
                    UserGroupId = roleEntity.UserGroupId,
                    IsDefault = roleEntity.IsDefault
                };
            }

            return null;
        }

        private HeadLightRoleMembership LoadModel(IMembershipRoleEntity membershipRoleEntity)
        {
            if (membershipRoleEntity != null)
            {
                return new HeadLightRoleMembership
                {
                    Id = membershipRoleEntity.Id,
                    MembershipId = membershipRoleEntity.MembershipId,
                    RoleId = membershipRoleEntity.RoleId,
                    DisplayName = membershipRoleEntity.DisplayName,
                    GivenName = membershipRoleEntity.GivenName,
                    SurName = membershipRoleEntity.SurName
                };
            }

            return null;
        }

        private IList<HeadLightRoleRight> LoadModels(IList<IRightEntity> rightEntities)
        {
            IList<HeadLightRoleRight> rights = new List<HeadLightRoleRight>();

            if (rightEntities != null)
            {
                foreach(IRightEntity rightEntity in rightEntities)
                {
                    rights.Add(LoadModel(rightEntity));
                }
            }

            return rights;
        }

        private IList<HeadLightRole> LoadModels(IList<IRoleEntity> roleEntities)
        {
            IList<HeadLightRole> roles = new List<HeadLightRole>();

            if (roleEntities != null)
            {
                foreach(IRoleEntity roleEntity in roleEntities)
                {
                    roles.Add(LoadModel(roleEntity));
                }

            }

            return roles;
        }

        private IList<HeadLightRoleMembership> LoadModels(IList<IMembershipRoleEntity> membershipRoleEntities)
        {
            IList<HeadLightRoleMembership> roleMemberships = new List<HeadLightRoleMembership>();

            if (membershipRoleEntities != null)
            {
                foreach(IMembershipRoleEntity membershipRoleEntity in membershipRoleEntities)
                {
                    roleMemberships.Add(LoadModel(membershipRoleEntity));
                }
            }

            return roleMemberships;
        }

        private readonly IRoleDataClient roleDataClient;
        private readonly ILogger<HeadLightRoleStore> logger;
        private readonly ILookupNormalizer normalizer;
    }
}