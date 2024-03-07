using Captivlink.Application.Users.Commands.Models;
using System.Collections.Generic;

namespace Captivlink.Api.Models.Requests
{
    public class PersonDetailsRequest
    {
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public List<SocialLinkModel> SocialMediaLinks { get; set; }
    }
}
