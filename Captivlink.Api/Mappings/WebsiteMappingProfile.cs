using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Application.Websites.Commands;
using Captivlink.Application.Websites.Queries;

namespace Captivlink.Api.Mappings
{
    public class WebsiteMappingProfile: Profile
    {
        public WebsiteMappingProfile()
        {
            CreateMap<WebsiteRequest, CreateWebsiteCommand>();
            CreateMap<WebsiteRequest, UpdateWebsiteCommand>();

            CreateMap<PaginationRequest, GetAllWebsiteQuery>();
        }
    }
}
