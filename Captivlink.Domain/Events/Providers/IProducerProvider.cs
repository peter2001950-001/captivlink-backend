namespace Captivlink.Infrastructure.Events.Providers
{
    public interface IProducerProvider
    {
        Task ProduceAsync(BaseEvent eventObj);
    }
}
