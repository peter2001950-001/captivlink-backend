using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Application.Users
{
    public class UserMappings :Profile
    {
        public UserMappings()
        {
            CreateMap<ApplicationUser, UserProfileResult>();
        }
    }
}
