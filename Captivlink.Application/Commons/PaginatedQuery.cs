using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Commons
{
    public abstract class PaginatedQuery<T> : IQuery<PaginatedResult<T>> where T : class
    {
        public int StartAt { get; set; } = 0;
        public int Count { get; set; } = 100;
        public IEnumerable<string>? SortFields { get; set; } = null;
    }
}
