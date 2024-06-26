﻿using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Application.Campaigns.Commands;
using Captivlink.Application.Campaigns.Queries;

namespace Captivlink.Api.Mappings
{
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            CreateMap<CampaignRequest, CreateCampaignCommand>();
            CreateMap<PaginationRequest, GetAllCampaignQuery>();
            CreateMap<PaginationRequest, GetCampaignFeedQuery>();
            CreateMap<PaginationRequest, GetCampaignPartnersQuery>();
            CreateMap<PaginationRequest, GetCampaignPartnersPerformanceQuery>();

            CreateMap<CampaignPartnershipRequest, GetCreatorCampaignsQuery>();

            CreateMap<CampaignRequest, UpdateCampaignCommand>();
        }
    }
}
