using System.Collections.Generic;
using System.Linq;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Attributes;
using Headlight.Models.Enumerations;
using Headlight.Models.Options;
using Headlight.Services.Authorization;
using Headlight.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Headlight
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SqlServerDataClientOptions>(Configuration.GetSection(SqlServerDataClientOptions.Section));
            services.Configure<LugOptions>(Configuration.GetSection(LugOptions.Section));
            services.Configure<SendGridOptions>(Configuration.GetSection(SendGridOptions.Section));
            
            services.AddScoped<IUserDataClient, SqlServerDataClient>();

            services.AddIdentity<HeadLightUser, HeadLightRole>()
                .AddUserStore<HeadLightUserStore>()
                .AddRoleStore<HeadLightRoleStore>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeAreaPage("User", "/Account/Logout");
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/User/Account/Login";
                options.LogoutPath = $"/User/Account/Logout";
                options.AccessDeniedPath = $"/User/Account/AccessDenied";
            });

            ConfigureAuthentication(services);
            ConfigureAuthorization(services);
#if DEBUG
            services.AddSingleton<IEmailService, LocalSmtpEmailService>();
#else
            services.AddSingleton<IEmailService, SendGridEmailService>();
#endif

            services.AddScoped<IUserGroupDataClient, SqlServerDataClient>();
            services.AddScoped<IMembershipDataClient, SqlServerDataClient>();
            services.AddScoped<IRoleDataClient, SqlServerDataClient>();

            services.AddScoped<HeadLightUserGroupStore>();
            services.AddScoped<HeadLightMembershipStore>();
            services.AddScoped<HeadLightRoleStore>();
            services.AddScoped<HeadLightUserStore>();

            services.AddScoped<IAuthorizationHandler, RequiredRightHandler>();
            services.AddScoped<IAuthorizationHandler, RequireAnyRightHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            AuthenticationBuilder builder = services.AddAuthentication();

            List<IConfigurationSection> sections = Configuration.GetSection(IdentityProvider.Section).GetChildren().ToList();

            foreach(IConfigurationSection section in sections)
            {
                IdentityProvider identityProvider = new IdentityProvider();
                section.Bind(identityProvider);

                switch(identityProvider.Type.ToLower())
                {
                    case "microsoft":
                        builder.AddMicrosoftAccount("Microsoft", identityProvider.DisplayName, options =>
                            {
                                options.ClientId = identityProvider.ClientId;
                                options.ClientSecret = identityProvider.ClientSecret;
                            });
                        break;

                    case "facebook":
                        builder.AddFacebook("Facebook", identityProvider.DisplayName, options =>
                            {
                                options.AppId = identityProvider.ClientId;
                                options.AppSecret = identityProvider.ClientSecret;
                            });
                        break;

                    case "twitter":
                        builder.AddTwitter("Twitter", identityProvider.DisplayName, options =>
                        {
                            options.ConsumerKey = identityProvider.ClientId;
                            options.ConsumerSecret = identityProvider.ClientSecret;
                            options.RetrieveUserDetails = true;
                        });
                        break;

                    case "google":
                        builder.AddGoogle("Google", identityProvider.DisplayName, options =>
                        {
                            options.ClientId = identityProvider.ClientId;
                            options.ClientSecret = identityProvider.ClientSecret;
                        });
                        break;

                    default:
                        break;
                }
            }
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Right.CreateRole.GetPolicyName(), policy => policy.Requirements.Add(new RequiredRightRequirement(Right.CreateRole)));
                options.AddPolicy(Right.UpdateRole.GetPolicyName(), policy => policy.Requirements.Add(new RequiredRightRequirement(Right.UpdateRole)));
                options.AddPolicy(Right.DeleteRole.GetPolicyName(), policy => policy.Requirements.Add(new RequiredRightRequirement(Right.DeleteRole)));
                options.AddPolicy(Right.MaintainUserGroupProfile.GetPolicyName(), policy => policy.Requirements.Add(new RequiredRightRequirement(Right.MaintainUserGroupProfile)));
                options.AddPolicy(Right.MaintainMemberships.GetPolicyName(), policy => policy.Requirements.Add(new RequiredRightRequirement(Right.MaintainMemberships)));

                options.AddPolicy("MaintainRoles", policy => policy.Requirements.Add(new RequireAnyRightRequirement(new List<Right> { Right.CreateRole, Right.UpdateRole, Right.DeleteRole })));
                options.AddPolicy("MaintainUserGroup", policy => policy.Requirements.Add(new RequireAnyRightRequirement(new List<Right> { Right.CreateRole, Right.UpdateRole, Right.DeleteRole, Right.MaintainUserGroupProfile, Right.MaintainMemberships })));
            });
        }
    }
}