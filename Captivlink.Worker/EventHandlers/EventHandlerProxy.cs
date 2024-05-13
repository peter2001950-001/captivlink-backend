using Captivlink.Worker.Interfaces;
using Newtonsoft.Json.Linq;

namespace Captivlink.Worker.EventHandlers
{
    public class EventHandlerProxy : IEventHandlerProxy
    {
        private readonly IEnumerable<IEventHandler> _handlers;

        public EventHandlerProxy(IEnumerable<IEventHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task HandleAsync(string value)
        {
            var type = JObject.Parse(value)["Type"]?.Value<string>();
            if(type == null) { return; }

            var handler = _handlers.FirstOrDefault(x => x.EventType == type);
            if(handler == null) { return; }

            await handler.HandleAsync(value);
        }
    }
}
