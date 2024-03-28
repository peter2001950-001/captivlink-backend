namespace Captivlink.Infrastructure.Utility
{
    public class PaginatedResult<T> where T : class
    {
        public PaginatedResult()
        {
            
        }

        public PaginatedResult(PaginationOptions options, int totalCount, IEnumerable<T> data)
        {
            StartAt = options.StartAt;
            Count = options.Count;
            SortFields = options.SortFields;
            TotalCount = totalCount;
            Data = data;
        }
        public int StartAt { get; set; } = 0;
        public int Count { get; set; } = 100;
        public IEnumerable<string>? SortFields { get; set; } = null;
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
