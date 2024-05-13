using Captivlink.Infrastructure.Events;

namespace Captivlink.Worker.Interfaces
{
    public interface IEventHandler
    {
        string EventType { get; }
        Task HandleAsync(string value);
    }
}
