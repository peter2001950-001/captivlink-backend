using Captivlink.Application.Websites.Results;

namespace Captivlink.Application.Websites.Commands
{
    public record UpdateWebsiteCommand : BaseWebsiteCommand<WebsiteResult>
    {
        public Guid Id { get; set; }
    }
}
