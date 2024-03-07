using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Application.Users.Commands.Models
{
    public class SocialLinkModel
    {
        public string Url { get; set; }
        public SocialLinkType Type { get; set; }
    }
}
