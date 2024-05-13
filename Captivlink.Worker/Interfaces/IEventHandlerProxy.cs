namespace Captivlink.Worker.Interfaces
{
    public interface IEventHandlerProxy
    {
        Task HandleAsync(string value);
    }
}
