using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Domain
{
    public class PersonDetails : Entity
    {
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<SocialLink> SocialMediaLinks { get; set; }
    }
}
