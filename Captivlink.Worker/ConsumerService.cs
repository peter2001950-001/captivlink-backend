using System.Diagnostics;
using Confluent.Kafka;
using System.Text.RegularExpressions;
using Captivlink.Infrastructure.Events;
using Captivlink.Worker.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Captivlink.Worker
{
    public class ConsumerService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventHandlerProxy _eventHandlerProxy;
        public ConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _eventHandlerProxy = serviceProvider.CreateScope().ServiceProvider.GetService<IEventHandlerProxy>()!;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = "worker",
                BootstrapServers = _configuration.GetConnectionString("Kafka"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            };

            try
            {
                using var consumerBuilder = new ConsumerBuilder
                    <string, string>(config).Build();

                consumerBuilder.Subscribe("events");
                var cancelToken = new CancellationTokenSource();

                try
                {
                    while (true)
                    {
                        var consumer = consumerBuilder.Consume(cancelToken.Token);
                        await _eventHandlerProxy.HandleAsync(consumer.Message.Value);

                        consumerBuilder.StoreOffset(consumer);

                        Debug.WriteLine($"Processing complete of request {consumer.Message.Key}");
                    }
                }
                catch (OperationCanceledException)
                {
                    consumerBuilder.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
