using System.Collections.Generic;
using System.Linq;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Options;
using Headlight.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
            
            services.AddScoped<IUserDataClient, SqlServerDataClient>();

            services.AddIdentity<HeadLightUser, HeadLightRole>()
                .AddUserStore<HeadLightUserStore>()
                .AddRoleStore<HeadLightRoleStore>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
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
            
#if DEBUG
                services.AddSingleton<IEmailSender, LocalSmtpMailSender>();
#else
                services.AddSingleton<IEmailSender, SendGridMailSender>();
#endif
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
                        break;

                    case "google":
                        break;

                    default:
                        break;
                }
            }
        }
    }
}