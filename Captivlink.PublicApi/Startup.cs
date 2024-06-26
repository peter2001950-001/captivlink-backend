using Captivlink.PublicApi.Utility.Swagger;
using System.Text.Json.Serialization;
using Captivlink.Infrastructure;
using Microsoft.IdentityModel.Logging;
using Captivlink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.PublicApi
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
                .AddMvcCore()
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

            if (Application.Switches.EnableSwagger)
            {
                services.AddSwaggerGen();
            }

            
            services.AddControllers();
            services.AddRepositories();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddServices();
            IdentityModelEventSource.ShowPII = true;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            if (Configuration.GetSection("Environment").Value == "prod")
            {
                app.UseHttpsRedirection();
            }

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
