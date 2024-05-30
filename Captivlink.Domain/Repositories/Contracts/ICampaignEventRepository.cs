using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Cache.Models;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Infrastructure.Repositories.Contracts
{
    public interface ICampaignEventRepository : IBaseRepository<CampaignEvent>
    {
    }
}
