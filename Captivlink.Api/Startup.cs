// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using Captivlink.Api.Data;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Captivlink.Infrastructure;
using Captivlink.Infrastructure.Repositories.Contracts;
using IdentityServer4.Extensions;
using Captivlink.Api.Utility;
using Captivlink.Api.Utility.Swagger;
using Captivlink.Application;
using System.Collections.Generic;

namespace Captivlink.Api
{
    public class BackendApiSwaggerOptions : SwaggerDefaultOptions
    {
        protected override string ApiTitle => "Captivlink Backend API";
    }

    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            Program.Application.Configuration = Configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // not recommended for production - you need to store your key material somewhere secure
            Seeder.RunDbMigrations(services).GetAwaiter().GetResult();
            RegisterIdentityServer(services);
            services.AddApplicationServices();

           
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });



        }

        private void RegisterIdentityServer(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddAuthentication()
                .AddLocalApi(options => { options.ExpectedScope = Program.Application.Authority.ApiName; });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });
            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.Authentication.CookieSlidingExpiration = true;
                options.Authentication.CookieLifetime = TimeSpan.FromMinutes(1);
            })
                .AddDeveloperSigningCredential()
                .AddConfigurationStore<AzureConfigurationDbContext>(options =>
                {
                    options.ConfigureDbContext = b =>
                    {
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore<AzurePersistedGrantDbContext>(options =>
                {
                    options.ConfigureDbContext = b =>
                    {
                        b.UseSqlServer(connectionString, sql => { sql.MigrationsAssembly(migrationsAssembly); });
                    };
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddTransient<IRedirectUriValidator, LogoutRedirectUriValidator>();
           
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequiredLength = 8;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters += "'";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddSwaggerConfig<BackendApiSwaggerOptions, AuthorizeCheckOperationFilter>(
                Program.Application.Authority,
                new Dictionary<string, string>
                {
                    { "captivlink-backend", "Captivlink Backend API" }
                }, disableSwagger: false);
            services.AddSwaggerGen();
            services.AddSession();
            services.AddApplicationServices();
        }

        public class IdentityWithAdditionalClaimsProfileService : IProfileService
        {
            private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
            private readonly IUserRepository _userRepository;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<ApplicationRole> _roleManager;

            public IdentityWithAdditionalClaimsProfileService(
                UserManager<ApplicationUser> userManager,
                RoleManager<ApplicationRole> roleManager,
                IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                IUserRepository userRepository)
            {
                this._userManager = userManager;
                this._roleManager = roleManager;
                this._claimsFactory = claimsFactory;
                this._userRepository = userRepository;
            }

            public async Task GetProfileDataAsync(ProfileDataRequestContext context)
            {
                var sub = context.Subject.GetSubjectId();
                if (!Guid.TryParse(sub, out var subGuid))
                    return;

                var user = await _userRepository.GetUserById(subGuid);
                if (user == null)
                    return;

                var principal = await _claimsFactory.CreateAsync(user);
                var claims = principal.Claims.ToList();

                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
                if (!string.IsNullOrWhiteSpace(user.FirstName))
                    claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                if (!string.IsNullOrWhiteSpace(user.LastName))
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
                    claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false"));
                }

                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
                    claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false"));
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Any())
                {
                    var role = await _roleManager.FindByNameAsync(roles.First());
                    if (role != null)
                    {
                        claims.Add(new Claim(JwtClaimTypes.Role, role.Name));
                        claims.Add(new Claim("role_id", role.Id.ToString()));
                    }
                }

                context.IssuedClaims = claims;
            }

            public async Task IsActiveAsync(IsActiveContext context)
            {
                var sub = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(sub);

                context.IsActive = user != null;
            }
        }

        public class LogoutRedirectUriValidator : StrictRedirectUriValidator
        {
            public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
            {
                var url = new Uri(requestedUri);
                var isStrictValid = await base.IsPostLogoutRedirectUriValidAsync(requestedUri, client);
                if (isStrictValid)
                    return true;

                var noQuery = url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
                return await base.IsPostLogoutRedirectUriValidAsync(noQuery, client);
            }


            public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
            {
                var url = new Uri(requestedUri);
                var isStrictValid = await base.IsRedirectUriValidAsync(requestedUri, client);
                if (isStrictValid)
                    return true;

                var noQuery = url.GetComponents(UriComponents.Scheme | UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.UriEscaped);
                return await base.IsRedirectUriValidAsync(noQuery.TrimEnd('/'), client);
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            Seeder.SeedDatabase(app.ApplicationServices).GetAwaiter().GetResult();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            app.Use(async (context, next) => {
                context.Response.Headers.Add("Content-Security-Policy", "default-src *; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; font-src 'self'; img-src http: https: data:; frame-src 'self'");

                await next();
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            if (Program.Application.Switches.EnableSwagger)
            {
                app.UseSwaggerForApi("captivlink-backend-swagger", opts =>
                {
                    opts.OAuthAppName("Captivlink Backend Swagger UI");
                });
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}