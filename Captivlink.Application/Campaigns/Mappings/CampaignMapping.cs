using AutoMapper;
using Captivlink.Application.Campaigns.Commands;
using Captivlink.Application.Campaigns.Commands.Models;
using Captivlink.Application.Campaigns.Queries;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Commons;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Mappings
{
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            CreateMap<CreateCampaignCommand, Campaign>()
                .ForMember(x => x.Categories, o => o.Ignore())
                .ForMember(x => x.Website, o => o.Ignore());

            CreateMap<AwardModel, Award>();

            CreateMap<Campaign, CampaignResult>();
            CreateMap<Award, AwardResult>();
            CreateMap<Category, CategoryResult>();

            CreateMap<GetAllCampaignQuery, PaginationOptions>();
        }

    }
}
