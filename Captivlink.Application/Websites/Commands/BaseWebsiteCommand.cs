using Captivlink.Application.Commons.Commands;

namespace Captivlink.Application.Websites.Commands
{
    public abstract record BaseWebsiteCommand<TResponse> : BaseCommand<TResponse> where TResponse: class
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool AllowSubdomains { get; set; }
    }
}
