using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Users.Results
{
    public class UserProfileResult
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public PersonResult PersonDetails { get; set; }
        public CompanyResult CompanyDetails { get; set; }

    }
}
