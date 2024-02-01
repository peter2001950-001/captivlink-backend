using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Captivlink.Backend
{
    public class Application
    {
        public class AuthorityConfig
        {
            public string BaseUrl { get; set; }
            public string AuthorizationUrl { get; set; }
            public string ApiName { get; set; }
            public string TokenUrl { get; set; }
        }

        public class SwitchesSection
        {
            public bool EnableSwagger { get; set; }
            public string SwaggerBaseUrl { get; set; }
        }

        public static IConfiguration Configuration { get; set; }
        public static SwitchesSection Switches => Configuration.GetSection("Switches").Get<SwitchesSection>();
        public static AuthorityConfig Authority => Configuration.GetSection("Authority").Get<AuthorityConfig>();
    }

    public class Program
    {

        public static int Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                Console.WriteLine("Application started!");

                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            Console.WriteLine("Application stopped!");

            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .ConfigureAppConfiguration((_, builder) =>
                        {
                            builder.Sources.Clear();
                            builder.SetBasePath(_.HostingEnvironment.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{_.HostingEnvironment.EnvironmentName}.json", optional: true)
                                .AddJsonFile($"appsettings.Local.json", optional: true)
                                .AddEnvironmentVariables();
                        })
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                            logging.AddConsole();
                            logging.AddDebug();
                            if (!hostingContext.HostingEnvironment.IsDevelopment())
                            {
                                //                                logging.AddApplicationInsights();
                            }
                        })
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .Enrich.FromLogContext()
                                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code);

                            if (!hostingContext.HostingEnvironment.IsDevelopment())
                            {
                                //                                loggerConfiguration.WriteTo.ApplicationInsights(TelemetryConverter.Traces);
                            }
                            else
                            {
                                loggerConfiguration.WriteTo.Debug();
                            }
                        })
                        .UseKestrel(c => c.AddServerHeader = false);
                });
    }


}