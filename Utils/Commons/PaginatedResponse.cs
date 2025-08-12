namespace ProyectTemplate.Utils.Commons
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasNext => Page * PageSize < TotalCount;
        public bool HasPrevious => Page > 1;
    }
}
