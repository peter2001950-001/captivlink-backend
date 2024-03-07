namespace Captivlink.Application.Users.Commands.Models
{
    public class PersonModel
    {
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public List<SocialLinkModel> SocialMediaLinks { get; set; }
    }
}
