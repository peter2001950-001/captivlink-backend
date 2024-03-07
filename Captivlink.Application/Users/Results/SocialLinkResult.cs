using Captivlink.Infrastructure.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Users.Results
{
    public class SocialLinkResult
    {
        public string Url { get; set; }
        public SocialLinkType Type { get; set; }
    }
}
