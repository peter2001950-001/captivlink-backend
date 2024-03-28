using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Websites.Commands;
using Captivlink.Application.Websites.Queries;
using Captivlink.Application.Websites.Results;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Websites.Mappings
{
    public class WebsiteMappingProfile : Profile
    {
        public WebsiteMappingProfile()
        {
            CreateMap<Website, WebsiteResult>();
            CreateMap<CreateWebsiteCommand, Website>();
            CreateMap<UpdateWebsiteCommand, Website>();

            CreateMap<GetAllWebsiteQuery, PaginationOptions>();
        }
    }
}
