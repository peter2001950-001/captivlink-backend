using System.Net.Mime;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Captivlink.Backend.Utility.Swagger
{
    public static class SwaggerExtensions
    {
        public static AuthenticationBuilder AddAuthenticationForApi(this IServiceCollection services,
            Application.AuthorityConfig authorityConfig, Action<IdentityServerAuthenticationOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authorityConfig.BaseUrl;
                    options.ApiName = authorityConfig.ApiName;
                    configureOptions?.Invoke(options);
                });
        }

        public static IServiceCollection AddSwaggerConfig<TSwaggerConfig, TAuthFilter>(this IServiceCollection services,
            Application.AuthorityConfig authorityConfig, IDictionary<string, string> scopes, bool disableSwagger = false)
            where TSwaggerConfig : class, IConfigureOptions<SwaggerGenOptions>
            where TAuthFilter : IOperationFilter
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));


            if (disableSwagger)
                return services;

            return services.AddTransient<IConfigureOptions<SwaggerGenOptions>, TSwaggerConfig>()
                .AddSwaggerGen(options =>
                {
                    options.DescribeAllParametersInCamelCase();
                    options.UseOneOfForPolymorphism();

                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri(authorityConfig?.TokenUrl),
                                Scopes = scopes,
                                AuthorizationUrl = new Uri(authorityConfig?.AuthorizationUrl)
                            }
                        }
                    });

                    options.OperationFilter<TAuthFilter>();


#pragma warning disable 618
                    //                    options.SchemaGeneratorOptions.DescribeAllEnumsAsStrings = true;
#pragma warning restore 618
                });
        }

        public static IApplicationBuilder UseSwaggerForApi(this IApplicationBuilder app,
            string clientId, Action<SwaggerUIOptions> configureOptions = null,
            bool disableSwagger = false, string baseUrl = null)
        {
            if (disableSwagger)
                return app;

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    var openApiServers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = baseUrl ?? $"https://{httpReq.Host.Value}" }
                    };

                    swagger.Servers = openApiServers;
                });
            });

            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");

                    options.RoutePrefix = string.Empty;

                    options.OAuthClientId(clientId);
                    options.OAuthAppName("API Swagger UI");

                    configureOptions?.Invoke(options);
                });

            return app;
        }
    }
}
