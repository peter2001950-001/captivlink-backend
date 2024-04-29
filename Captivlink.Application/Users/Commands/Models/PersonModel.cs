namespace Captivlink.Application.Users.Commands.Models
{
    public class PersonModel
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Nationality { get; set; }
        public List<SocialLinkModel> SocialMediaLinks { get; set; }
        public List<Guid> Categories { get; set; }
    }
}
