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
            CreateMap<PersonDetails, PersonResult>();
            CreateMap<SocialLink, SocialLinkResult>();

            CreateMap<PersonModel, PersonDetails>();
            CreateMap<SocialLinkModel, SocialLink>();

            CreateMap<CompanyModel, CompanyDetails>();
        }
    }
}
