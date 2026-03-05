namespace Blog.ViewModels.Common
{
    public class PaginationQuery
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 25;

        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = DefaultPageSize;

        public string? Search { get; set; }

        public void Normalize()
        {
            if (Page < 0)
                Page = 0;

            if (PageSize < 1 || PageSize > MaxPageSize)
                PageSize = DefaultPageSize;

            if (!string.IsNullOrWhiteSpace(Search))
                Search = Search.Trim().ToLower();
        }
    }
}