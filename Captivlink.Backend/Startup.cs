using Captivlink.Backend.Utility.Swagger;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Logging;

namespace Captivlink.Backend
{
    public class Startup
    {
        public class PublicApiSwaggerOptions : SwaggerDefaultOptions
        {

            protected override string ApiTitle => "Captivlink Backend";
        }

        public Startup(IConfiguration configuration)
        {
            Application.Configuration = Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors()
                .AddCors()
                .AddMvcCore(
                //    options =>
                //{
                //    options.Filters.Add(new CustomValidationAttribute());
                //}
                    )
                .AddApiExplorer()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, true));
                })
                .ConfigureApplicationPartManager(a =>
                {
                    var appPart = a.ApplicationParts.FirstOrDefault(ap => ap.Name == "Captivlink.Api");
                    if (appPart != null)
                    {
                        a.ApplicationParts.Remove(appPart);
                    }
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewAccounts", policy =>
                    policy.RequireClaim("client_access_level", "account"));
                options.AddPolicy("MakeDraftPayments", policy =>
                    policy.RequireClaim("client_access_level", "draft_payment"));
            });

            //services
            //    .AddDbContext<ApplicationDbContext>(options =>
            //    {
            //        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            //            provider => provider.EnableRetryOnFailure());
            //    });

            var authorityConfig = Configuration.GetSection("Authority").Get<Application.AuthorityConfig>();
            services.AddAuthenticationForApi(authorityConfig);

            var switches = Configuration.GetSection("Switches").Get<Application.SwitchesSection>();

            if (switches.EnableSwagger)
            {
                services.AddSwaggerConfig<PublicApiSwaggerOptions, AuthorizeCheckOperationFilter>(authorityConfig,
                    new Dictionary<string, string>
                {
                    {"captivlink-backend", "Captivlink Backend"},
                    {"openid", "Captivlink Backend"},
                    {"profile", "Captivlink Backend"}
                }, !switches.EnableSwagger);
            }

            services.AddControllers();

            IdentityModelEventSource.ShowPII = true;
            //
            // services.AddSwaggerConfig<PublicApiSwaggerOptions, AuthorizeCheckOperationFilter>(
            //     Application.Authority,
            //     new Dictionary<string, string>
            //     {
            //         {"moneyfold-api", "Moneyfold Public API"}
            //     }, false, disableSwagger: false);
            //
            // services.AddMvcCore()
            //     .AddAuthorization()
            //     .ConfigureApplicationPartManager(a =>
            //     {
            //         var appPart = a.ApplicationParts.FirstOrDefault(ap => ap.Name == "Moneyfold.API");
            //         if (appPart != null)
            //         {
            //             a.ApplicationParts.Remove(appPart);
            //         }
            //     })
            //     .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            if (Application.Switches.EnableSwagger)
            {
                app.UseSwaggerForApi("captivlink-backend-swagger", opts =>
                {
                    opts.OAuthAppName("Captivlink Backend Swagger UI");
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
