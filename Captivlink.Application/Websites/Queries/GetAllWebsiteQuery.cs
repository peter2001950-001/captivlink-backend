using Captivlink.Application.Commons;
using Captivlink.Application.Websites.Results;

namespace Captivlink.Application.Websites.Queries
{
    public class GetAllWebsiteQuery : PaginatedQuery<WebsiteResult>
    {
        public Guid UserId { get; set; }
    }
}
