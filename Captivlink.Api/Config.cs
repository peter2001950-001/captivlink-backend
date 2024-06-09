// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Api
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("captivlink-backend"),
            };

        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("captivlink-backend", "Captivlink Backend API")
                {
                    Scopes = new List<string>(){"captivlink-backend"}
                },
                new ApiResource("captivlink-api", "Captivlink Public API"),
            };
        }

        public static IEnumerable<ApplicationRole> GetRoles()
        {
            return new[]
            {
                new ApplicationRole
                {
                    Id = new Guid("50d3f7b8-2b9e-4e17-b57b-92581a0c30d7"),
                    Name = "ContentCreator",
                    NormalizedName = "CONTENTCREATOR",
                    ConcurrencyStamp = "dc45397b-4968-416c-abaf-586bbd68d22e"
                },
                new ApplicationRole
                {
                    Id = new Guid("9d5fc5df-37d0-4eef-bafa-cb6909f71a0e"),
                    Name = "Business",
                    NormalizedName = "BUSINESS",
                    ConcurrencyStamp = "5b3bb423-297f-408a-827a-3a6d5743b7b4"
                }
            };
        }

        public static IEnumerable<Client> GetClientsForEnvironment(string environment)
        {
            var clientBaseUrls = GetUrlsForEnvironment(environment);

            var uiBaseUrls = clientBaseUrls.FirstOrDefault(x => x.Key == "frontend").Value;
            var backendBaseUrls = clientBaseUrls.FirstOrDefault(x => x.Key == "api").Value;
            var adminBaseUrls = clientBaseUrls.FirstOrDefault(x => x.Key == "admin").Value;

            return new Client[]
            {
                new Client
                {
                    ClientId = "captivlink-backend-swagger",
                    ClientName = "Captivlink Backend Swagger UI",
                    ClientUri = $"{backendBaseUrls.First()}/index.html",
                    RequireClientSecret = false,

                    AllowedGrantTypes = new[] {GrantType.Implicit},
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = (int) Math.Floor(TimeSpan.FromHours(1).TotalSeconds),

                    RedirectUris = backendBaseUrls.SelectMany(x => new[]
                    {
                        $"{x}/index.html",
                        $"{x}/oauth2-redirect.html",
                    }).ToList(),

                    PostLogoutRedirectUris = backendBaseUrls.ToList(),
                    AllowedCorsOrigins = backendBaseUrls.ToList(),

                    AllowedScopes = {"openid", "profile", "email", "captivlink-backend"}
                },
                new Client
                {
                    ClientId = "captivlink-ui",
                    ClientName = "Captivlink UI",
                    ClientUri = $"{uiBaseUrls.First()}",
                    RequireClientSecret = false,

                    AllowedGrantTypes = new[] {GrantType.AuthorizationCode},
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = (int) Math.Floor(TimeSpan.FromMinutes(5).TotalSeconds),
                    SlidingRefreshTokenLifetime = (int) Math.Floor(TimeSpan.FromMinutes(15).TotalSeconds),
                    AbsoluteRefreshTokenLifetime = (int) Math.Floor(TimeSpan.FromMinutes(30).TotalSeconds),
                    RedirectUris = uiBaseUrls.SelectMany(x => new[]
                    {
                        $"{x}/signin-callback",
                        $"{x}/silent-renew.html",
                    }).ToList(),

                    PostLogoutRedirectUris = new List<string>(){ $"{uiBaseUrls.First()}/signout-callback"},
                    AllowedCorsOrigins = uiBaseUrls.ToList(),

                    AllowedScopes = {"openid", "profile", "email", "captivlink-backend"}
                },
            };
        }

        private static Dictionary<string, string[]> GetUrlsForEnvironment(string environment)
        {
            switch (environment)
            {
                case "prod":
                    return new Dictionary<string, string[]>
                    {
                        {"frontend", new[] {"https://portal.captivlink.com", "https://green-desert-0d4411c0f.5.azurestaticapps.net"}},
                        {"backend", new[] {"https://ctiv.me"}},
                        {"api", new[] {"https://api.captivlink.com"}},
                    };
                default:
                    return new Dictionary<string, string[]>
                    {
                        {"frontend", new[] {"http://localhost:4200"}},
                        {"backend", new[] {"https://localhost:5100"}},
                        {"api", new[] {"https://localhost:5001"}},
                    };
            }
        }
    }

}