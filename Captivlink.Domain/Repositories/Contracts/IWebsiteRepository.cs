using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain;

namespace Captivlink.Infrastructure.Repositories.Contracts
{
    public interface IWebsiteRepository : IBaseRepository<Website>
    {
        Task<bool> IsWebsiteExisting(Website website);
    }
}
