using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Users.Results
{
    public class PersonResult
    {
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public List<SocialLinkResult> SocialMediaLinks { get; set; }
    }
}
