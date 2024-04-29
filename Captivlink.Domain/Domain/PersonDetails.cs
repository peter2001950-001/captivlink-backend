using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Domain
{
    public class PersonDetails : Entity
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public IReadOnlyCollection<SocialLink> SocialMediaLinks { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
