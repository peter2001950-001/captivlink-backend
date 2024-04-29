using AutoMapper;
using Captivlink.Application.Users.Commands.Models;
using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Application.Users.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<ApplicationUser, UserProfileResult>()
                .ForMember(x => x.PersonDetails, o=>o.MapFrom(t => t.Person))
                .ForMember(x => x.CompanyDetails, o=>o.MapFrom(t => t.Company));
            CreateMap<CompanyDetails, CompanyResult>();
            CreateMap<PersonDetails, PersonResult>()
                .ForMember(x => x.Nationality, m=>m.MapFrom(o => o.Country))
                .ForMember(x => x.Categories, o=>o.MapFrom(m => m.Categories));
            CreateMap<SocialLink, SocialLinkResult>();

            CreateMap<PersonModel, PersonDetails>()
                .ForMember(x => x.Categories, o=>o.Ignore())
                .ForMember(x=>x.Country, m=>m.MapFrom(o=>o.Nationality));
            CreateMap<SocialLinkModel, SocialLink>();

            CreateMap<CompanyModel, CompanyDetails>();
        }
    }
}
