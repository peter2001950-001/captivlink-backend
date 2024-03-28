using System.Collections.Generic;

namespace Captivlink.Api.Models.Requests
{
    public class PaginationRequest
    {
        public int StartAt { get; set; } = 0;
        public int Count { get; set; } = 100;
        public IEnumerable<string>? SortFields { get; set; } = null;
    }
}
