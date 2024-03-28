using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Websites.Results
{
    public class WebsiteResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public bool AllowSubdomains { get; set; }
        public string AccessToken { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
