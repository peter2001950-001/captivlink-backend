using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Commons.Commands;

namespace Captivlink.Application.Websites.Commands
{
    public record DeleteWebsiteCommand : BaseCommand<bool>
    {
        public Guid UserId { get; set; }
        public Guid WebsiteId { get; set; }
    }
}
