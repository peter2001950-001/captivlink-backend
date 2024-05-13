using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value) where T : class;
        Task<T?> GetAsync<T>(string key) where T : class;
        Task DeleteAsync(string key);
    }
}
