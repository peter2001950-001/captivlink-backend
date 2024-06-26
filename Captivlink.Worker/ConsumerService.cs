using System.Diagnostics;
using Captivlink.Infrastructure;
using Confluent.Kafka;
using Captivlink.Worker.Interfaces;

namespace Captivlink.Worker
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventHandlerProxy _eventHandlerProxy;
        public ConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _eventHandlerProxy = serviceProvider.CreateScope().ServiceProvider.GetService<IEventHandlerProxy>()!;
        }

        protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            var config = new ConsumerConfig(DependencyInjection.KafkaClientConfig)
            {
                GroupId = "worker",
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

                        Console.WriteLine($"Processing complete of request {consumer.Message.Key}");
                    }
                }
                catch (OperationCanceledException)
                {
                    consumerBuilder.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
