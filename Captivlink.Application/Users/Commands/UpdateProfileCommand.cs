using Captivlink.Application.Commons.Commands;
using Captivlink.Application.Users.Commands.Models;

namespace Captivlink.Application.Users.Commands
{
    public record UpdateProfileCommand : BaseCommand<bool>
    {
        public Guid UserId { get; set; }
        public string UserRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PersonModel? PersonDetails { get; set; }
        public CompanyModel? CompanyDetails { get; set; }
    }
}
