using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Captivlink.Infrastructure.Events.Providers
{
    public class ProducerProvider : IProducerProvider
    {
        private readonly IProducer<string, string> _producer;

        public ProducerProvider(IConfiguration configuration)
        {
            var producerconfig = new ProducerConfig(DependencyInjection.KafkaClientConfig);

            _producer = new ProducerBuilder<string, string>(producerconfig).Build();
        }

        public async Task ProduceAsync(BaseEvent eventObj)
        {
            var serialize = JsonConvert.SerializeObject(eventObj);

            await _producer.ProduceAsync("events",
                new Message<string, string>()
                    {Key = eventObj.Id.ToString(), Timestamp = new Timestamp(eventObj.CreatedOn), Value = serialize});

        }
    }

}

