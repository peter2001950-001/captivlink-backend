namespace Captivlink.Api.Models.Requests
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public PersonDetailsRequest PersonDetails { get; set; }

        public CompanyDetailsRequest CompanyDetails { get; set; }
    }
}
