namespace Captivlink.Infrastructure.Events
{
    public abstract class BaseEvent<T> : BaseEvent where T: class
    {
        public T Data { get; set; }

    }

    public abstract class BaseEvent
    {
        public Guid Id { get; set; }
        public abstract string Type { get; }
        public string SessionId { get; set; }
        public string? IpAddress { get; set; }
        public string AffCode { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
