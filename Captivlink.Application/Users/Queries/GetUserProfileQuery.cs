using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Users.Results;

namespace Captivlink.Application.Users.Queries
{
    public class GetUserProfileQuery : IQuery<UserProfileResult?>
    {
        public Guid UserId { get; set; }
    }
}
