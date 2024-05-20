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

            CreateMap<Campaign, CampaignBusinessResult>();
            CreateMap<Campaign, CampaignCreatorResult>();

            CreateMap<Award, AwardResult>();
            CreateMap<Category, CategoryResult>();
            CreateMap<CompanyDetails, CompanyResult>();

            CreateMap<GetAllCampaignQuery, PaginationOptions>();
            CreateMap<GetCampaignFeedQuery, PaginationOptions>();
            CreateMap<GetCampaignPartnersQuery, PaginationOptions>();
            CreateMap<GetCreatorCampaignsQuery, PaginationOptions>();
            CreateMap<GetCampaignPartnersPerformanceQuery, PaginationOptions>();

            CreateMap<CampaignPartner, CampaignPartnerResult>()
                .ForMember(x=>x.ContentCreator, o => o.MapFrom(m => m.ContentCreator));
        }

    }
}
