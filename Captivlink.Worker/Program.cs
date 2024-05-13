using Captivlink.Infrastructure;
using Captivlink.Worker.EventHandlers;
using Captivlink.Worker.Interfaces;

namespace Captivlink.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddRepositories();
            builder.Services.AddSingleton<IHostedService, ConsumerService>();
            builder.Services.AddScoped<IEventHandlerProxy, EventHandlerProxy>();
            builder.Services.AddScoped<IEventHandler, ClickEventHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();


            app.Run();
        }
    }
}
