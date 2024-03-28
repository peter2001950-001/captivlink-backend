namespace Captivlink.Api.Models.Requests
{
    public class WebsiteRequest
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool AllowSubdomains { get; set; }
    }
}
