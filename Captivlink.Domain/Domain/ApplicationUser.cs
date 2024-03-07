using Microsoft.AspNetCore.Identity;

namespace Captivlink.Infrastructure.Domain
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public CompanyDetails? Company { get; set; }
        public PersonDetails? Person { get; set; }
    }
}
