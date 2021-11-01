using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Headlight.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Headlight.Data
{
    public class SqlServerDataClient : IUserDataClient, IUserGroupDataClient, IMembershipDataClient, IRoleDataClient
    {
        public SqlServerDataClient(IOptions<SqlServerDataClientOptions> options, ILogger<SqlServerDataClient> logger)
        {
            connectionString = options.Value.ConnectionString;
            this.logger = logger;
        }

        #region IUserDataClient Implementation

        public async Task CreateLoginAsync(ILoginEntity loginEntity, CancellationToken cancellationToken = new())
        {
            logger.LogInformation("Entered CreateLoginAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateLogin]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter providerIdParameter = new SqlParameter("@providerId", SqlDbType.NVarChar, 100) { Value = loginEntity.ProviderId };
            command.Parameters.Add(providerIdParameter);
            logger.LogTrace($"Parameter {providerIdParameter.ParameterName} of type {providerIdParameter.SqlDbType} has value {providerIdParameter.Value}");

            SqlParameter providerKeyParameter = new SqlParameter("@providerKey", SqlDbType.NVarChar, 100) { Value = loginEntity.ProviderKey };
            command.Parameters.Add(providerKeyParameter);
            logger.LogTrace($"Parameter {providerKeyParameter.ParameterName} of type {providerKeyParameter.SqlDbType} has value {providerKeyParameter.Value}");

            SqlParameter providerDisplayNameParameter = new SqlParameter("@providerDisplayName", SqlDbType.NVarChar, 100) { Value = loginEntity.ProviderDisplayName };
            command.Parameters.Add(providerDisplayNameParameter);
            logger.LogTrace($"Parameter {providerDisplayNameParameter.ParameterName} of type {providerDisplayNameParameter.SqlDbType} has value {providerDisplayNameParameter.Value}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = loginEntity.UserId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving CreateLoginAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateLoginAsync");
            }
        }

        public async Task CreateUserAsync(IUserEntity userEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateUserAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateUser]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} with direction {idParameter.Direction}");

            SqlParameter displayNameParameter = new SqlParameter("@displayName", SqlDbType.NVarChar, 100) { Value = userEntity.DisplayName };
            command.Parameters.Add(displayNameParameter);
            logger.LogTrace($"Parameter {displayNameParameter.ParameterName} of type {displayNameParameter.SqlDbType} has value {displayNameParameter.Value}");

            SqlParameter emailParameter = new SqlParameter("@email", SqlDbType.NVarChar, 320) { Value = userEntity.Email };
            command.Parameters.Add(emailParameter);
            logger.LogTrace($"Parameter {emailParameter.ParameterName} of type {emailParameter.SqlDbType} has value {emailParameter.Value}");

            SqlParameter normalizedEmailParameter = new SqlParameter("@normalizedEmail", SqlDbType.NVarChar, 320) { Value = userEntity.NormalizedEmail };
            command.Parameters.Add(normalizedEmailParameter);
            logger.LogTrace($"Parameter {normalizedEmailParameter.ParameterName} of type {normalizedEmailParameter.SqlDbType} has value {normalizedEmailParameter.Value}");

            SqlParameter emailConfirmedParameter = new SqlParameter("@emailConfirmed", SqlDbType.Bit) { Value = userEntity.EmailConfirmed };
            command.Parameters.Add(emailConfirmedParameter);
            logger.LogTrace($"Parameter {emailConfirmedParameter.ParameterName} of type {emailConfirmedParameter.SqlDbType} has value {emailConfirmedParameter.Value}");

            SqlParameter givenNameParameter = new SqlParameter("@givenName", SqlDbType.NVarChar, 100) { Value = userEntity.GivenName ?? "" };
            command.Parameters.Add(givenNameParameter);
            logger.LogTrace($"Parameter {givenNameParameter.ParameterName} of type {givenNameParameter.SqlDbType} has value {givenNameParameter.Value}");

            SqlParameter surNameParameter = new SqlParameter("@surName", SqlDbType.NVarChar, 100) { Value = userEntity.SurName ?? "" };
            command.Parameters.Add(surNameParameter);
            logger.LogTrace($"Parameter {surNameParameter.ParameterName} of type {surNameParameter.SqlDbType} has value {surNameParameter.Value}");

            SqlParameter phoneNumberParameter = new SqlParameter("@phoneNumber", SqlDbType.NVarChar, 24) { Value = userEntity.PhoneNumber ?? "" };
            command.Parameters.Add(phoneNumberParameter);
            logger.LogTrace($"Parameter {phoneNumberParameter.ParameterName} of type {phoneNumberParameter.SqlDbType} has value {phoneNumberParameter.Value}");

            SqlParameter phoneNumberConfirmedParameter = new SqlParameter("@phoneNumberConfirmed", SqlDbType.Bit) { Value = userEntity.PhoneNumberConfirmed };
            command.Parameters.Add(phoneNumberConfirmedParameter);
            logger.LogTrace($"Parameter {phoneNumberConfirmedParameter.ParameterName} of type {phoneNumberConfirmedParameter.SqlDbType} has value {phoneNumberConfirmedParameter.Value}");

            SqlParameter streetAddressLineOneParameter = new SqlParameter("@streetAddressLine1", SqlDbType.NVarChar, 100) { Value = userEntity.StreetAddressLine1 ?? "" };
            command.Parameters.Add(streetAddressLineOneParameter);
            logger.LogTrace($"Parameter {streetAddressLineOneParameter.ParameterName} of type {streetAddressLineOneParameter.SqlDbType} has value {streetAddressLineOneParameter.Value}");

            SqlParameter streetAddressLineTwoParameter = new SqlParameter("@streetAddressLine2", SqlDbType.NVarChar, 100) { Value = userEntity.StreetAddressLine2 ?? "" };
            command.Parameters.Add(streetAddressLineTwoParameter);
            logger.LogTrace($"Parameter {streetAddressLineTwoParameter.ParameterName} of type {streetAddressLineTwoParameter.SqlDbType} has value {streetAddressLineTwoParameter.Value}");

            SqlParameter cityParameter = new SqlParameter("@city", SqlDbType.NVarChar, 100) { Value = userEntity.City ?? "" };
            command.Parameters.Add(cityParameter);
            logger.LogTrace($"Parameter {cityParameter.ParameterName} of type {cityParameter.SqlDbType} has value {cityParameter.Value}");

            SqlParameter regionParameter = new SqlParameter("@region", SqlDbType.NVarChar, 100) { Value = userEntity.Region ?? "" };
            command.Parameters.Add(regionParameter);
            logger.LogTrace($"Parameter {regionParameter.ParameterName} of type {regionParameter.SqlDbType} has value {regionParameter.Value}");

            SqlParameter countryParameter = new SqlParameter("@country", SqlDbType.NVarChar, 100) { Value = userEntity.Country ?? "" };
            command.Parameters.Add(countryParameter);
            logger.LogTrace($"Parameter {countryParameter.ParameterName} of type {countryParameter.SqlDbType} has value {countryParameter.Value}");

            SqlParameter postalCodeParameter = new SqlParameter("@postalCode", SqlDbType.NVarChar, 100) { Value = userEntity.PostalCode ?? "" };
            command.Parameters.Add(postalCodeParameter);
            logger.LogTrace($"Parameter {postalCodeParameter.ParameterName} of type {postalCodeParameter.SqlDbType} has value {postalCodeParameter.Value}");

            SqlParameter dateOfBirthParameter = new SqlParameter("@dateOfBirth", SqlDbType.Date) { Value = userEntity.DateOfBirth };
            command.Parameters.Add(dateOfBirthParameter);
            logger.LogTrace($"Parameter {dateOfBirthParameter.ParameterName} of type {dateOfBirthParameter.SqlDbType} has value {dateOfBirthParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                userEntity.Id = (long)idParameter.Value;
                logger.LogInformation("Successfully Leaving CreateUserAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateUserAsync");
                throw;
            }
        }

        public async Task DeleteLoginAsync(ILoginEntity loginEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteLoginAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteLogin]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter providerIdParameter = new SqlParameter("@providerId", SqlDbType.NVarChar, 100) { Value = loginEntity.ProviderId };
            command.Parameters.Add(providerIdParameter);
            logger.LogTrace($"Parameter {providerIdParameter.ParameterName} of type {providerIdParameter.SqlDbType} has value {providerIdParameter.Value}");

            SqlParameter providerUserKeyParameter = new SqlParameter("@providerKey", SqlDbType.NVarChar, 100) { Value = loginEntity.ProviderKey };
            command.Parameters.Add(providerUserKeyParameter);
            logger.LogTrace($"Parameter {providerUserKeyParameter.ParameterName} of type {providerUserKeyParameter.SqlDbType} has value {providerUserKeyParameter.Value}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = loginEntity.UserId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteLoginAsync");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in DeleteLoginAsync");
            }
        }

        public async Task DeleteUserAsync(IUserEntity userEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteUserAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteUser]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = userEntity.Id };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteUserAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in DeleteUserAsync");
                throw;
            }
        }

        public async Task<IList<ILoginEntity>> RetrieveLoginsByUserIdAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveLoginsByUserIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveLoginByUserId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

                List<ILoginEntity> logins = new List<ILoginEntity>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    string loginProvider = reader.GetString("ProviderId");
                    string providerDisplayName = reader.GetString("ProviderDisplayName");
                    string providerKey = reader.GetString("ProviderKey");

                    logins.Add(new LoginEntity
                    {
                        ProviderId = loginProvider,
                        ProviderKey = providerKey,
                        ProviderDisplayName = providerDisplayName,
                        UserId = userId
                    });
                }

                logger.LogInformation("Successfully Leaving RetrieveLoginsByUserIdAsync");
                return logins;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveLoginsByUserIdAsync");
                throw;
            }
        }

        public async Task<IUserEntity> RetrieveUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserByEmailAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserByEmail]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter normalizedEmailParameter = new SqlParameter("@normalizedEmail", SqlDbType.NVarChar, 320) { Value = normalizedEmail };
            command.Parameters.Add(normalizedEmailParameter);
            logger.LogTrace($"Parameter {normalizedEmailParameter.ParameterName} of type {normalizedEmailParameter.SqlDbType} has value {normalizedEmailParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserEntity userEntity = await LoadUserEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserByEmailAsync");

                return userEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserByEmailAsync");
                throw;
            }
        }

        public async Task<IUserEntity> RetrieveUserByLoginAsync(string providerId, string providerKey, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserByLoginAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserByLogin]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter providerIdParameter = new SqlParameter("@providerId", SqlDbType.NVarChar, 100) { Value = providerId };
            command.Parameters.Add(providerIdParameter);
            logger.LogTrace($"Parameter {providerIdParameter.ParameterName} of type {providerIdParameter.SqlDbType} has value {providerIdParameter.Value}");

            SqlParameter providerKeyParameter = new SqlParameter("@providerKey", SqlDbType.NVarChar, 100) { Value = providerKey };
            command.Parameters.Add(providerKeyParameter);
            logger.LogTrace($"Parameter {providerKeyParameter.ParameterName} of type {providerKeyParameter.SqlDbType} has value {providerKeyParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserEntity userEntity = await LoadUserEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserByLoginAsync");

                return userEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserByLoginAsync");
                throw;
            }
        }

        public async Task<IUserEntity> RetrieveUserByUserIdAsync(long userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveUserByLoginAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserByUserId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserEntity userEntity = await LoadUserEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserByLoginAsync");

                return userEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserByLoginAsync");
                throw;
            }
        }

        public async Task<IUserEntity> RetrieveUserByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveUserByUserNameAsync");

            try
            {
                return await RetrieveUserByEmailAsync(normalizedUserName, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserByUserNameAsync");
                throw;
            }
        }

        public async Task UpdateUserAsync(IUserEntity userEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered UpdateUserAsync");

            await using SqlConnection conn = new SqlConnection(connectionString); 
            await using SqlCommand command = new SqlCommand("[dbo].[UpdateUser]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = userEntity.Id };
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} has value {idParameter.Value}");

            SqlParameter displayNameParameter = new SqlParameter("displayName", SqlDbType.NVarChar, 100) { Value = userEntity.DisplayName };
            command.Parameters.Add(displayNameParameter);
            logger.LogTrace($"Parameter {displayNameParameter.ParameterName} of type {displayNameParameter.SqlDbType} has value {displayNameParameter.Value}");

            SqlParameter isActiveParameter = new SqlParameter("isActive", SqlDbType.Bit) { Value = userEntity.IsActive };
            command.Parameters.Add(isActiveParameter);
            logger.LogTrace($"Parameter {isActiveParameter.ParameterName} of type {isActiveParameter.SqlDbType} has value {isActiveParameter.Value}");

            SqlParameter isApprovedParameter = new SqlParameter("isApproved", SqlDbType.Bit) { Value = userEntity.IsApproved };
            command.Parameters.Add(isApprovedParameter);
            logger.LogTrace($"Parameter {isApprovedParameter.ParameterName} of type {isApprovedParameter.SqlDbType} has value {isApprovedParameter.Value}");

            SqlParameter emailParameter = new SqlParameter("email", SqlDbType.NVarChar, 320) { Value = userEntity.Email };
            command.Parameters.Add(emailParameter);
            logger.LogTrace($"Parameter {emailParameter.ParameterName} of type {emailParameter.SqlDbType} has value {emailParameter.Value}");

            SqlParameter normalizedEmailParameter = new SqlParameter("normalizedEmail", SqlDbType.NVarChar, 320) { Value = userEntity.NormalizedEmail };
            command.Parameters.Add(normalizedEmailParameter);
            logger.LogTrace($"Parameter {normalizedEmailParameter.ParameterName} of type {normalizedEmailParameter.SqlDbType} has value {normalizedEmailParameter.Value}");

            SqlParameter emailConfirmedParameter = new SqlParameter("emailConfirmed", SqlDbType.Bit) { Value = userEntity.EmailConfirmed };
            command.Parameters.Add(emailConfirmedParameter);
            logger.LogTrace($"Parameter {emailConfirmedParameter.ParameterName} of type {emailConfirmedParameter.SqlDbType} has value {emailConfirmedParameter.Value}");

            SqlParameter givenNameParameter = new SqlParameter("givenName", SqlDbType.NVarChar, 100) { Value = userEntity.GivenName ?? "" };
            command.Parameters.Add(givenNameParameter);
            logger.LogTrace($"Parameter {givenNameParameter.ParameterName} of type {givenNameParameter.SqlDbType} has value {givenNameParameter.Value}");

            SqlParameter surNameParameter = new SqlParameter("surName", SqlDbType.NVarChar, 100) { Value = userEntity.SurName ?? "" };
            command.Parameters.Add(surNameParameter);
            logger.LogTrace($"Parameter {surNameParameter.ParameterName} of type {surNameParameter.SqlDbType} has value {surNameParameter.Value}");

            SqlParameter phoneNumberParameter = new SqlParameter("phoneNumber", SqlDbType.NVarChar, 24) { Value = userEntity.PhoneNumber ?? "" };
            command.Parameters.Add(phoneNumberParameter);
            logger.LogTrace($"Parameter {phoneNumberParameter.ParameterName} of type {phoneNumberParameter.SqlDbType} has value {phoneNumberParameter.Value}");

            SqlParameter phoneNumberConfirmedParameter = new SqlParameter("phoneNumberConfirmed", SqlDbType.Bit) { Value = userEntity.PhoneNumberConfirmed };
            command.Parameters.Add(phoneNumberConfirmedParameter);
            logger.LogTrace($"Parameter {phoneNumberConfirmedParameter.ParameterName} of type {phoneNumberConfirmedParameter.SqlDbType} has value {phoneNumberConfirmedParameter.Value}");

            SqlParameter streetAddressLineOneParameter = new SqlParameter("streetAddressLine1", SqlDbType.NVarChar, 100) { Value = userEntity.StreetAddressLine1 ?? "" };
            command.Parameters.Add(streetAddressLineOneParameter);
            logger.LogTrace($"Parameter {streetAddressLineOneParameter.ParameterName} of type {streetAddressLineOneParameter.SqlDbType} has value {streetAddressLineOneParameter.Value}");

            SqlParameter streetAddressLineTwoParameter = new SqlParameter("streetAddressLine2", SqlDbType.NVarChar, 100) { Value = userEntity.StreetAddressLine2 ?? "" };
            command.Parameters.Add(streetAddressLineTwoParameter);
            logger.LogTrace($"Parameter {streetAddressLineTwoParameter.ParameterName} of type {streetAddressLineTwoParameter.SqlDbType} has value {streetAddressLineTwoParameter.Value}");

            SqlParameter cityParameter = new SqlParameter("city", SqlDbType.NVarChar, 100) { Value = userEntity.City ?? "" };
            command.Parameters.Add(cityParameter);
            logger.LogTrace($"Parameter {cityParameter.ParameterName} of type {cityParameter.SqlDbType} has value {cityParameter.Value}");

            SqlParameter regionParameter = new SqlParameter("region", SqlDbType.NVarChar, 100) { Value = userEntity.Region ?? "" };
            command.Parameters.Add(regionParameter);
            logger.LogTrace($"Parameter {regionParameter.ParameterName} of type {regionParameter.SqlDbType} has value {regionParameter.Value}");

            SqlParameter countryParameter = new SqlParameter("country", SqlDbType.NVarChar, 100) { Value = userEntity.Country ?? "" };
            command.Parameters.Add(countryParameter);
            logger.LogTrace($"Parameter {countryParameter.ParameterName} of type {countryParameter.SqlDbType} has value {countryParameter.Value}");

            SqlParameter postalCodeParameter = new SqlParameter("postalCode", SqlDbType.NVarChar, 100) { Value = userEntity.PostalCode ?? "" };
            command.Parameters.Add(postalCodeParameter);
            logger.LogTrace($"Parameter {postalCodeParameter.ParameterName} of type {postalCodeParameter.SqlDbType} has value {postalCodeParameter.Value}");

            SqlParameter dateOfBirthParameter = new SqlParameter("dateOfBirth", SqlDbType.Date) { Value = userEntity.DateOfBirth };
            command.Parameters.Add(dateOfBirthParameter);
            logger.LogTrace($"Parameter {dateOfBirthParameter.ParameterName} of type {dateOfBirthParameter.SqlDbType} has value {dateOfBirthParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);

                logger.LogInformation("Successfully Leaving UpdateUserAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateUserAsync");
                throw;
            }
        }

        private async Task<IUserEntity> LoadUserEntity(SqlDataReader reader, CancellationToken cancellationToken = new CancellationToken())
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                return new UserEntity
                {
                    Id = reader.GetInt64("Id"),
                    DisplayName = reader.GetString("DisplayName"),
                    IsActive = reader.GetBoolean("IsActive"),
                    IsApproved = reader.GetBoolean("IsApproved"),
                    Email = reader.GetString("Email"),
                    NormalizedEmail = reader.GetString("NormalizedEmail"),
                    EmailConfirmed = reader.GetBoolean("EmailConfirmed"),
                    GivenName = reader.GetString("GivenName"),
                    SurName = reader.GetString("SurName"),
                    PhoneNumber = reader.GetString("PhoneNumber"),
                    PhoneNumberConfirmed = reader.GetBoolean("PhoneNumberConfirmed"),
                    StreetAddressLine1 = reader.GetString("StreetAddressLine1"),
                    StreetAddressLine2 = reader.GetString("StreetAddressLine2"),
                    City = reader.GetString("City"),
                    Region = reader.GetString("Region"),
                    Country = reader.GetString("Country"),
                    PostalCode = reader.GetString("PostalCode"),
                    DateOfBirth = reader.GetDateTime("DateOfBirth")
                };
            }

            return null;
        }

        #endregion

        #region IUserGroupDataClient Implementation

        public async Task CreateUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered CreateUserGroupAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateUserGroup]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} with direction {idParameter.Direction}");

            SqlParameter fullNameParameter = new SqlParameter("@fullName", SqlDbType.NVarChar, 256) { Value = userGroupEntity.FullName };
            command.Parameters.Add(fullNameParameter);
            logger.LogTrace($"Parameter {fullNameParameter.ParameterName} of type {fullNameParameter.SqlDbType} has value {fullNameParameter.Value}");

            SqlParameter joinTypeParameter = new SqlParameter("@joinType", SqlDbType.TinyInt) { Value = userGroupEntity.JoinType };
            command.Parameters.Add(joinTypeParameter);
            logger.LogTrace($"Parameter {joinTypeParameter.ParameterName} of type {joinTypeParameter.SqlDbType} has value {joinTypeParameter.Value}");

            SqlParameter normalizedFullNameParameter = new SqlParameter("@normalizedFullName", SqlDbType.NVarChar, 256) { Value = userGroupEntity.NormalizedFullName };
            command.Parameters.Add(normalizedFullNameParameter);
            logger.LogTrace($"Parameter {normalizedFullNameParameter.ParameterName} of type {normalizedFullNameParameter.SqlDbType} has value {normalizedFullNameParameter.Value}");

            SqlParameter shortNameParameter = new SqlParameter("@shortName", SqlDbType.NVarChar, 64) { Value = userGroupEntity.ShortName };
            command.Parameters.Add(shortNameParameter);
            logger.LogTrace($"Parameter {shortNameParameter.ParameterName} of type {shortNameParameter.SqlDbType} has value {shortNameParameter.Value}");

            SqlParameter normalizedShortNameParameter = new SqlParameter("@normalizedShortName", SqlDbType.NVarChar, 64) { Value = userGroupEntity.NormalizedShortName };
            command.Parameters.Add(normalizedShortNameParameter);
            logger.LogTrace($"Parameter {normalizedShortNameParameter.ParameterName} of type {normalizedShortNameParameter.SqlDbType} has value {normalizedShortNameParameter.Value}");

            SqlParameter slackWorkspaceIdParameter = new SqlParameter("@slackWorkspaceId", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(userGroupEntity.SlackWorkspaceId) ? DBNull.Value : userGroupEntity.SlackWorkspaceId };
            command.Parameters.Add(slackWorkspaceIdParameter);
            logger.LogTrace($"Parameter {slackWorkspaceIdParameter.ParameterName} of type {slackWorkspaceIdParameter.SqlDbType} has value {slackWorkspaceIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                userGroupEntity.Id = (long)idParameter.Value;
                logger.LogInformation("Successfully Leaving CreateUserGroupAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateUserGroupAsync");
                throw;
            }
        }

        public async Task DeleteUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered DeleteUserGroupAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteUserGroup]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userGroupIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = userGroupEntity.Id };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteUserGroupAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in DeleteUserGroupAsync");
                throw;
            }
        }

        public async Task<IUserGroupEntity> RetrieveUserGroupAsync(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered RetrieveUserGroupAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserGroup]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserGroupEntity userGroupEntity = await LoadUserGroupEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserGroupAsync");

                return userGroupEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserGroupAsync");
                throw;
            }
        }

        public async Task<IUserGroupEntity> RetrieveUserGroupByFullNameAsync(string normalizedFullName, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered RetrieveUserGroupByFullNameAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserGroupByFullName]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter normalizedFullNameParameter = new SqlParameter("@normalizedFullName", SqlDbType.NVarChar, 256) { Value = normalizedFullName };
            command.Parameters.Add(normalizedFullNameParameter);
            logger.LogTrace($"Parameter {normalizedFullNameParameter.ParameterName} of type {normalizedFullNameParameter.SqlDbType} has value {normalizedFullNameParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserGroupEntity userGroupEntity = await LoadUserGroupEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserGroupByFullNameAsync");

                return userGroupEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserGroupByFullNameAsync");
                throw;
            }
        }

        public async Task<IUserGroupEntity> RetrieveUserGroupByShortNameAsync(string normalizedShortName, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered RetrieveUserGroupByShortNameAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserGroupByShortName]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter normalizedShortNameParameter = new SqlParameter("@normalizedShortName", SqlDbType.NVarChar, 64) { Value = normalizedShortName };
            command.Parameters.Add(normalizedShortNameParameter);
            logger.LogTrace($"Parameter {normalizedShortNameParameter.ParameterName} of type {normalizedShortNameParameter.SqlDbType} has value {normalizedShortNameParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserGroupEntity userGroupEntity = await LoadUserGroupEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserGroupByShortNameAsync");

                return userGroupEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserGroupByShortNameAsync");
                throw;
            }
        }

        public async Task<IUserGroupEntity> RetrieveUserGroupByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered RetrieveUserGroupByUserGroupIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserGroupByUserGroupId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = userGroupId };
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} has value {idParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IUserGroupEntity userGroupEntity = await LoadUserGroupEntity(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveUserGroupByUserGroupIdAsync");

                return userGroupEntity;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserGroupByUserGroupIdAsync");
                throw;
            }
        }

        public async Task<IList<IUserGroupEntity>> RetrieveUserGroupsByJoinTypeAsync(byte joinTypes, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered RetrieveUserGroupsByJoinTypeAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveUserGroupsByJoinType]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter joinTypeParameter = new SqlParameter("@joinType", SqlDbType.TinyInt) { Value = joinTypes };
            command.Parameters.Add(joinTypeParameter);
            logger.LogTrace($"Parameter {joinTypeParameter.ParameterName} of type {joinTypeParameter.SqlDbType} has value {joinTypeParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IList<IUserGroupEntity> userGroupEntities = await LoadUserGroupEntities(reader, cancellationToken);
                
                logger.LogInformation("Successfully Leaving RetrieveUserGroupByJoinTypeAsync");

                return userGroupEntities;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in RetrieveUserGroupByJoinTypeAsync");
                throw;
            }
        }

        public async Task UpdateUserGroupAsync(IUserGroupEntity userGroupEntity, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Entered UpdateUserGroupAsync");

            await using SqlConnection conn = new SqlConnection(connectionString); 
            await using SqlCommand command = new SqlCommand("[dbo].[UpdateUserGroup]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt){ Value = userGroupEntity.Id};
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} has value {idParameter.Value}");

            SqlParameter fullNameParameter = new SqlParameter("@fullName", SqlDbType.NVarChar, 256) { Value = userGroupEntity.FullName };
            command.Parameters.Add(fullNameParameter);
            logger.LogTrace($"Parameter {fullNameParameter.ParameterName} of type {fullNameParameter.SqlDbType} has value {fullNameParameter.Value}");

            SqlParameter joinTypeParameter = new SqlParameter("@joinType", SqlDbType.TinyInt) { Value = userGroupEntity.JoinType };
            command.Parameters.Add(joinTypeParameter);
            logger.LogTrace($"Parameter {joinTypeParameter.ParameterName} of type {joinTypeParameter.SqlDbType} has value {joinTypeParameter.Value}");

            SqlParameter normalizedFullNameParameter = new SqlParameter("@normalizedFullName", SqlDbType.NVarChar, 256) { Value = userGroupEntity.NormalizedFullName };
            command.Parameters.Add(normalizedFullNameParameter);
            logger.LogTrace($"Parameter {normalizedFullNameParameter.ParameterName} of type {normalizedFullNameParameter.SqlDbType} has value {normalizedFullNameParameter.Value}");

            SqlParameter shortNameParameter = new SqlParameter("@shortName", SqlDbType.NVarChar, 64) { Value = userGroupEntity.ShortName };
            command.Parameters.Add(shortNameParameter);
            logger.LogTrace($"Parameter {shortNameParameter.ParameterName} of type {shortNameParameter.SqlDbType} has value {shortNameParameter.Value}");

            SqlParameter normalizedShortNameParameter = new SqlParameter("@normalizedShortName", SqlDbType.NVarChar, 64) { Value = userGroupEntity.NormalizedShortName };
            command.Parameters.Add(normalizedShortNameParameter);
            logger.LogTrace($"Parameter {normalizedShortNameParameter.ParameterName} of type {normalizedShortNameParameter.SqlDbType} has value {normalizedShortNameParameter.Value}");

            SqlParameter slackWorkspaceIdParameter = new SqlParameter("@slackWorkspaceId", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(userGroupEntity.SlackWorkspaceId) ? DBNull.Value : userGroupEntity.SlackWorkspaceId };
            command.Parameters.Add(slackWorkspaceIdParameter);
            logger.LogTrace($"Parameter {slackWorkspaceIdParameter.ParameterName} of type {slackWorkspaceIdParameter.SqlDbType} has value {slackWorkspaceIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);

                logger.LogInformation("Successfully Leaving UpdateUserGroupAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateUserGroupAsync");
                throw;
            }
        }

        private async Task<IUserGroupEntity> LoadUserGroupEntity(SqlDataReader reader, CancellationToken cancellationToken = new CancellationToken())
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                return new UserGroupEntity
                {
                    Id = reader.GetInt64("Id"),
                    FullName = reader.GetString("FullName"),
                    JoinType = reader.GetByte("JoinType"),
                    NormalizedFullName = reader.GetString("NormalizedFullName"),
                    ShortName = reader.GetString("ShortName"),
                    NormalizedShortName = reader.GetString("NormalizedShortName"),
                    SlackWorkspaceId = await reader.IsDBNullAsync("SlackWorkspaceId", cancellationToken)
                        ? null
                        : reader.GetString("SlackWorkspaceId")
                };
            }

            return null;
        }

        private async Task<List<IUserGroupEntity>> LoadUserGroupEntities(SqlDataReader reader, CancellationToken cancellationToken = new CancellationToken())
        {
            List<IUserGroupEntity> result = new List<IUserGroupEntity>();

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new UserGroupEntity
                {
                    Id = reader.GetInt64("Id"),
                    FullName = reader.GetString("FullName"),
                    JoinType = reader.GetByte("JoinType"),
                    NormalizedFullName = reader.GetString("NormalizedFullName"),
                    ShortName = reader.GetString("ShortName"),
                    NormalizedShortName = reader.GetString("NormalizedShortName"),
                    SlackWorkspaceId = await reader.IsDBNullAsync("SlackWorkspaceId", cancellationToken)
                        ? null
                        : reader.GetString("SlackWorkspaceId")
                });
            }

            return result;
        }

        #endregion

        #region IMembershipDataClient Implementaion

        public async Task CreateMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered CreateMembershipAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateMembership]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter idParameter = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(idParameter);
            logger.LogTrace($"Parameter {idParameter.ParameterName} of type {idParameter.SqlDbType} with direction {idParameter.Direction}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = membershipEntity.UserId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            SqlParameter userGroupIdParameter = new SqlParameter("@userGroupId", SqlDbType.BigInt) { Value = membershipEntity.UserGroupId };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            SqlParameter isActiveParameter = new SqlParameter("@isActive", SqlDbType.Bit) { Value = membershipEntity.IsActive };
            command.Parameters.Add(isActiveParameter);
            logger.LogTrace($"Parameter {isActiveParameter.ParameterName} of type {isActiveParameter.SqlDbType} has value {isActiveParameter.Value}");

            SqlParameter slackMembershipIdParameter = new SqlParameter("@slackMemberId", SqlDbType.NVarChar, 256) { Value = string.IsNullOrWhiteSpace(membershipEntity.SlackMemberId) ? DBNull.Value : membershipEntity.SlackMemberId };
            command.Parameters.Add(slackMembershipIdParameter);
            logger.LogTrace($"Parameter {slackMembershipIdParameter.ParameterName} of type {slackMembershipIdParameter.SqlDbType} has value {slackMembershipIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                membershipEntity.Id = (long)idParameter.Value;
                logger.LogInformation("Successfully Leaving CreateMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateMembershipAsync");
                throw;
            }
        }

        public async Task DeleteMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered DeleteMembershipAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteMembership]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter membershipIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = membershipEntity.Id };
            command.Parameters.Add(membershipIdParameter);
            logger.LogTrace($"Parameter {membershipIdParameter.ParameterName} of type {membershipIdParameter.SqlDbType} has value {membershipIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in DeleteMembershipAsync");
                throw;
            }
        }

        public async Task<IUserGroupEntity> RetrieveCurrentUserGroupAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveCurrentUserGroupAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipsByUserId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IMembershipEntity currentMembership = (await LoadMembershipEntities(reader, cancellationToken)).FirstOrDefault(m => m.IsCurrent == true);
                IUserGroupEntity result = await RetrieveUserGroupByUserGroupIdAsync(currentMembership.UserGroupId, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveCurrentUserGroupAsync");
                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveCurrentUserGroupAsync");

                return null;
            }
        }

        public async Task<IMembershipEntity> RetrieveMembershipByMembershipIdAsync(long membershipId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipByMembershipIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipByMembershipId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter membershipIdParameter = new SqlParameter("@membershipId", SqlDbType.BigInt) { Value = membershipId };
            command.Parameters.Add(membershipIdParameter);
            logger.LogTrace($"Parameter {membershipIdParameter.ParameterName} of type {membershipIdParameter.SqlDbType} has value {membershipIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IMembershipEntity result = await LoadMembershipEntity(reader, cancellationToken);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveMembershipByMembershipIdAsync");

                return null;
            }
        }

        public async Task<IList<IUserGroupEntity>> RetrieveUserGroupsByUserIdAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveUserGroupsByUserIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipsByUserId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                List<IMembershipEntity> memberships = await LoadMembershipEntities(reader, cancellationToken);

                List<IUserGroupEntity> result = new List<IUserGroupEntity>();

                foreach(IMembershipEntity membership in memberships)
                {
                    result.Add(await RetrieveUserGroupByUserGroupIdAsync(membership.UserGroupId, cancellationToken));
                }
                
                logger.LogInformation("Successfully Leaving RetrieveUserGroupsByUserIdAsync");
                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveUserGroupsByUserIdAsync");

                return null;
            }
        }

        public async Task<IList<IMembershipEntity>> RetrieveMembershipsByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipsByUserGroupIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipsByUserGroupId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userGroupIdParameter = new SqlParameter("@userGroupId", SqlDbType.BigInt) { Value = userGroupId };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                List<IMembershipEntity> memberships = await LoadMembershipEntities(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveMembershipsByUserGroupIdAsync");
                return memberships;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveMembershipsByUserGroupIdAsync");

                return null;
            }
        }

        public async Task<IList<IMembershipEntity>> RetrieveMembershipsByUserIdAsync(long userId, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered RetrieveMembershipsByUserIdAsnyc");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipsByUserId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userIdParameter = new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId };
            command.Parameters.Add(userIdParameter);
            logger.LogTrace($"Parameter {userIdParameter.ParameterName} of type {userIdParameter.SqlDbType} has value {userIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                List<IMembershipEntity> memberships = await LoadMembershipEntities(reader, cancellationToken);

                logger.LogInformation("Successfully Leaving RetrieveMembershipsByUserIdAsnyc");
                return memberships;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveMembershipsByUserIdAsnyc");

                return null;
            }
        }

        public async Task UpdateMembershipAsync(IMembershipEntity membershipEntity, CancellationToken cancellationToken = new ())
        {
            logger.LogInformation("Entered UpdateMembershipAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[UpdateMembership]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter membershipIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = membershipEntity.Id };
            command.Parameters.Add(membershipIdParameter);
            logger.LogTrace($"Parameter {membershipIdParameter.ParameterName} of type {membershipIdParameter.SqlDbType} has value {membershipIdParameter.Value}");

            SqlParameter isActiveParameter = new SqlParameter("@isActive", SqlDbType.Bit) { Value = membershipEntity.IsActive };
            command.Parameters.Add(isActiveParameter);
            logger.LogTrace($"Parameter {isActiveParameter.ParameterName} of type {isActiveParameter.SqlDbType} has value {isActiveParameter.Value}");

            SqlParameter isCurrentParameter = new SqlParameter("@isCurrent", SqlDbType.Bit) { Value = membershipEntity.IsCurrent };
            command.Parameters.Add(isCurrentParameter);
            logger.LogTrace($"Parameter {isCurrentParameter.ParameterName} of type {isCurrentParameter.SqlDbType} has value {isCurrentParameter.Value}");

            SqlParameter isPrimaryParameter = new SqlParameter("@isPrimary", SqlDbType.Bit) { Value = membershipEntity.IsPrimary };
            command.Parameters.Add(isPrimaryParameter);
            logger.LogTrace($"Parameter {isPrimaryParameter.ParameterName} of type {isPrimaryParameter.SqlDbType} has value {isPrimaryParameter.Value}");

            SqlParameter slackMemberIdParameter = new SqlParameter("@slackMemberId", SqlDbType.NVarChar, 256)
            {
                Value = (object)membershipEntity.SlackMemberId ?? DBNull.Value
            };
            command.Parameters.Add(slackMemberIdParameter);
            logger.LogTrace($"Parameter {slackMemberIdParameter.ParameterName} of type {slackMemberIdParameter.SqlDbType} has value {slackMemberIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving UpdateMembershipAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateMembershipAsync");
                throw;
            }
        }

        private async Task<List<IMembershipEntity>> LoadMembershipEntities(SqlDataReader reader, CancellationToken cancellationToken = new ())
        {
            List<IMembershipEntity> results = new List<IMembershipEntity>();

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(
                    new MembershipEntity
                    {
                        Id = reader.GetInt64("Id"),
                        UserId = reader.GetInt64("UserId"),
                        UserGroupId = reader.GetInt64("UserGroupId"),
                        IsActive = reader.GetBoolean("IsActive"),
                        IsCurrent = reader.GetBoolean("IsCurrent"),
                        IsPrimary = reader.GetBoolean("IsPrimary"),
                        SlackMemberId = await reader.IsDBNullAsync("SlackMemberId", cancellationToken)
                        ? null
                        : reader.GetString("SlackMemberId"),
                        DisplayName = reader.GetString("DisplayName"),
                        GivenName = reader.GetString("GivenName"),
                        SurName = reader.GetString("SurName")
                    }
                );
            }

            return results;
        }

        private async Task<IMembershipEntity> LoadMembershipEntity(SqlDataReader reader, CancellationToken cancellationToken = new ())
        {
            IMembershipEntity result = null;

            if (await reader.ReadAsync(cancellationToken))
            {
                result = new MembershipEntity
                    {
                        Id = reader.GetInt64("Id"),
                        UserId = reader.GetInt64("UserId"),
                        UserGroupId = reader.GetInt64("UserGroupId"),
                        IsActive = reader.GetBoolean("IsActive"),
                        IsCurrent = reader.GetBoolean("IsCurrent"),
                        IsPrimary = reader.GetBoolean("IsPrimary"),
                        SlackMemberId = await reader.IsDBNullAsync("SlackMemberId", cancellationToken)
                        ? null
                        : reader.GetString("SlackMemberId"),
                        DisplayName = reader.GetString("DisplayName"),
                        GivenName = reader.GetString("GivenName"),
                        SurName = reader.GetString("SurName")
                    };
            }

            return result;
        }

        #endregion

        #region IRoleDataClient Implementation

        public async Task CreateMembershipRoleAsync(IMembershipRoleEntity membershipRoleEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered CreateMembershipRoleAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateMembershipRole]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter membershipRoleIdParameter = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(membershipRoleIdParameter);
            logger.LogTrace($"Parameter {membershipRoleIdParameter.ParameterName} of type {membershipRoleIdParameter.SqlDbType} with direction {membershipRoleIdParameter.Direction}");

            SqlParameter membershipIdParameter = new SqlParameter("@membershipId", SqlDbType.BigInt) { Value = membershipRoleEntity.MembershipId };
            command.Parameters.Add(membershipIdParameter);
            logger.LogTrace($"Parameter {membershipIdParameter.ParameterName} of type {membershipIdParameter.SqlDbType} has value {membershipIdParameter.Value}");

            SqlParameter roleIdParameter = new SqlParameter("@roleId", SqlDbType.BigInt) { Value = membershipRoleEntity.RoleId };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                membershipRoleEntity.Id = (long)membershipRoleIdParameter.Value;
                logger.LogInformation("Successfully Leaving CreateMembershipRoleAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateMembershipRoleAsync");
                throw;
            }
        }

        public async Task CreateRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered CreateRoleAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[CreateRole]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} with direction {roleIdParameter.Direction}");

            SqlParameter nameParameter = new SqlParameter("@roleName", SqlDbType.NVarChar, 256) { Value = roleEntity.Name };
            command.Parameters.Add(nameParameter);
            logger.LogTrace($"Parameter {nameParameter.ParameterName} of type {nameParameter.SqlDbType} has value {nameParameter.Value}");

            SqlParameter normalizedNameParameter = new SqlParameter("@normalizedRoleName", SqlDbType.NVarChar, 256) { Value = roleEntity.NormalizedName };
            command.Parameters.Add(normalizedNameParameter);
            logger.LogTrace($"Parameter {normalizedNameParameter.ParameterName} of type {normalizedNameParameter.SqlDbType} has value {normalizedNameParameter.Value}");

            SqlParameter userGroupIdParameter = new SqlParameter("@userGroupId", SqlDbType.BigInt) { Value = roleEntity.UserGroupId };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            SqlParameter isDefaultParameter = new SqlParameter("@isDefault", SqlDbType.Bit) { Value = roleEntity.IsDefault };
            command.Parameters.Add(isDefaultParameter);
            logger.LogTrace($"Parameter {isDefaultParameter.ParameterName} of type {isDefaultParameter.SqlDbType} has value {isDefaultParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                roleEntity.Id = (long)roleIdParameter.Value;
                logger.LogInformation("Successfully Leaving CreateRoleAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in CreateRoleAsync");
                throw;
            }
        }

        public async Task DeleteMembershipRoleAsync(IMembershipRoleEntity membershipRoleEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered DeleteMembershipRoleAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteMembershipRole]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter membershipRoleIdParameter = new SqlParameter("@membershipRoleId", SqlDbType.BigInt) { Value = membershipRoleEntity.Id };
            command.Parameters.Add(membershipRoleIdParameter);
            logger.LogTrace($"Parameter {membershipRoleIdParameter.ParameterName} of type {membershipRoleIdParameter.SqlDbType} has value {membershipRoleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteMembershipRoleAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in DeleteMembershipRoleAsync");
                throw;
            }
        }

        public async Task DeleteRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered DeleteRoleAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[DeleteRole]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = roleEntity.Id };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving DeleteRoleAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in DeleteRoleAsync");
                throw;
            }
        }

        public async Task<IList<IMembershipRoleEntity>> RetrieveMembershipRolesByRoleId(long roleId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveMembershipRolesByRoleIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveMembershipRolesByRoleId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger .LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@roleId", SqlDbType.BigInt) { Value = roleId };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IList<IMembershipRoleEntity> result = await LoadMembershipRoleEntities(reader, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RetrieveMembershipRolesByRoleIdAsync");

                return null;
            }
        }

        public async Task<IList<IRightEntity>> RetrieveRightsByRoleIdAsync(long roleId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveRightsByRoleIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveRightsByRoleId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger .LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@roleId", SqlDbType.BigInt) { Value = roleId };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IList<IRightEntity> result = await LoadRightEntities(reader, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RetrieveRightsByRoleIdAsync");

                return null;
            }
        }

        public async Task<IRoleEntity> RetrieveRoleByRoleIdAsync(long roleId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveRoleByRoleIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveRoleByRoleId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@roleId", SqlDbType.BigInt) { Value = roleId };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IRoleEntity result = await LoadRoleEntity(reader, cancellationToken);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveRoleByRoleIdAsync");

                return null;
            }
        }

        public async Task<IList<IRoleEntity>> RetrieveRolesByUserGroupIdAsync(long userGroupId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered RetrieveRolesByUserGroupIdAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[RetrieveRolesByUserGroupId]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter userGroupIdParameter = new SqlParameter("@userGroupId", SqlDbType.BigInt) { Value = userGroupId };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
                IList<IRoleEntity> result = await LoadRoleEntities(reader, cancellationToken);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in RetrieveRoleByRoleIdAsync");

                return null;
            }
        }

        public async Task UpdateRoleAsync(IRoleEntity roleEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered UpdateRoleAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[UpdateRole]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleIdParameter = new SqlParameter("@id", SqlDbType.BigInt) { Value = roleEntity.Id };
            command.Parameters.Add(roleIdParameter);
            logger.LogTrace($"Parameter {roleIdParameter.ParameterName} of type {roleIdParameter.SqlDbType} has value {roleIdParameter.Value}");

            SqlParameter nameParameter = new SqlParameter("@roleName", SqlDbType.NVarChar, 256) { Value = roleEntity.Name };
            command.Parameters.Add(nameParameter);
            logger.LogTrace($"Parameter {nameParameter.ParameterName} of type {nameParameter.SqlDbType} has value {nameParameter.Value}");

            SqlParameter normalizedNameParameter = new SqlParameter("@normalizedRoleName", SqlDbType.NVarChar, 256) { Value = roleEntity.NormalizedName };
            command.Parameters.Add(normalizedNameParameter);
            logger.LogTrace($"Parameter {normalizedNameParameter.ParameterName} of type {normalizedNameParameter.SqlDbType} has value {normalizedNameParameter.Value}");

            SqlParameter userGroupIdParameter = new SqlParameter("@userGroupId", SqlDbType.BigInt) { Value = roleEntity.UserGroupId };
            command.Parameters.Add(userGroupIdParameter);
            logger.LogTrace($"Parameter {userGroupIdParameter.ParameterName} of type {userGroupIdParameter.SqlDbType} has value {userGroupIdParameter.Value}");

            SqlParameter isDefaultParameter = new SqlParameter("@isDefault", SqlDbType.Bit) { Value = roleEntity.IsDefault };
            command.Parameters.Add(isDefaultParameter);
            logger.LogTrace($"Parameter {isDefaultParameter.ParameterName} of type {isDefaultParameter.SqlDbType} has value {isDefaultParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving UpdateRoleAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateRoleAsync");
                throw;
            }
        }

        public async Task UpdateRoleRightAsync(IRightEntity rightEntity, CancellationToken cancellationToken)
        {
            logger.LogInformation("Entered UpdateRoleRightAsync");

            await using SqlConnection conn = new SqlConnection(connectionString);
            await using SqlCommand command = new SqlCommand("[dbo].[UpdateRoleRight]", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            logger.LogTrace($"Preparing to call stored procedure: {command.CommandText}");

            SqlParameter roleRightIdParameter = new SqlParameter("@roleRightId", SqlDbType.BigInt) { Value = rightEntity.Id };
            command.Parameters.Add(roleRightIdParameter);
            logger.LogTrace($"Parameter {roleRightIdParameter.ParameterName} of type {roleRightIdParameter.SqlDbType} has value {roleRightIdParameter.Value}");

            SqlParameter stateParameter = new SqlParameter("@state", SqlDbType.Bit) { Value = rightEntity.State == null ? DBNull.Value : rightEntity.State};
            command.Parameters.Add(stateParameter);
            logger.LogTrace($"Parameter {stateParameter.ParameterName} of type {stateParameter.SqlDbType} has value {stateParameter.Value}");

            await conn.OpenAsync(cancellationToken);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                logger.LogInformation("Successfully Leaving UpdateRoleRightAsync");
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error in UpdateRoleRightAsync");
                throw;
            }
        }

        private async Task<List<IMembershipRoleEntity>> LoadMembershipRoleEntities(SqlDataReader reader, CancellationToken cancellationToken = new())
        {
            List<IMembershipRoleEntity> result = new List<IMembershipRoleEntity>();

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(
                    new MembershipRoleEntity
                    {
                        Id = reader.GetInt64("Id"),
                        MembershipId = reader.GetInt64("MembershipId"),
                        RoleId = reader.GetInt64("RoleId"),
                        DisplayName = reader.GetString("DisplayName"),
                        GivenName = reader.GetString("GivenName"),
                        SurName = reader.GetString("SurName")
                    }
                 );
            }

            return result;
        }

        private async Task<List<IRightEntity>> LoadRightEntities(SqlDataReader reader, CancellationToken cancellationToken = new ())
        {
            List<IRightEntity> results = new List<IRightEntity>();

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(
                    new RightEntity
                    {
                        Id = reader.GetInt64("Id"),
                        Name = reader.GetString("RightName"),
                        State = await reader.IsDBNullAsync("State", cancellationToken) ? null : reader.GetBoolean("State")
                    }
                );
            }

            return results;
        }

        private async Task<List<IRoleEntity>> LoadRoleEntities(SqlDataReader reader, CancellationToken cancellationToken = new ())
        {
            List<IRoleEntity> results = new List<IRoleEntity>();

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(
                    new RoleEntity
                    {
                        Id = reader.GetInt64("Id"),
                        Name = reader.GetString("RoleName"),
                        NormalizedName = reader.GetString("NormalizedRoleName"),
                        UserGroupId = reader.GetInt64("UserGroupId"),
                        IsDefault = reader.GetBoolean("IsDefault")
                    }
                );
            }

            return results;
        }

        private async Task<IRoleEntity> LoadRoleEntity(SqlDataReader reader, CancellationToken cancellationToken = new ())
        {
            IRoleEntity result = null;

            if (await reader.ReadAsync(cancellationToken))
            {
                result = new RoleEntity
                    {
                        Id = reader.GetInt64("Id"),
                        Name = reader.GetString("RoleName"),
                        NormalizedName = reader.GetString("NormalizedRoleName"),
                        UserGroupId = reader.GetInt64("UserGroupId"),
                        IsDefault = reader.GetBoolean("IsDefault")
                    };
            }

            return result;
        }
        #endregion

        private readonly string connectionString;
        private readonly ILogger<SqlServerDataClient> logger;
    }
}