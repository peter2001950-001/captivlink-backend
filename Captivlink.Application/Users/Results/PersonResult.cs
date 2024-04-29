using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Commons;

namespace Captivlink.Application.Users.Results
{
    public class PersonResult
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Nationality { get; set; }
        public List<SocialLinkResult> SocialMediaLinks { get; set; }
        public List<CategoryResult> Categories { get; set; }
    }
}
