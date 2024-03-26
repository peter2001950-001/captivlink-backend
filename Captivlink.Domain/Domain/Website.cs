using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Domain
{
    public class Website : Entity
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool AllowSubdomains { get; set; }
        public string AccessToken { get; set; }
    }
}
