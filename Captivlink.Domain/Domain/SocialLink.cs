using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Infrastructure.Domain
{
    public record SocialLink
    {
        public string Url { get; set; }
        public  SocialLinkType Type { get; set; }
    }
}
