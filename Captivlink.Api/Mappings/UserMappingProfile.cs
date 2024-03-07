using AutoMapper;
using Captivlink.Api.Models.Requests;
using Captivlink.Application.Users.Commands;
using Captivlink.Application.Users.Commands.Models;

namespace Captivlink.Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<PersonDetailsRequest, PersonModel>();
            CreateMap<CompanyDetailsRequest, CompanyModel>();
            CreateMap<UpdateProfileRequest, UpdateProfileCommand>();
        }
    }
}
