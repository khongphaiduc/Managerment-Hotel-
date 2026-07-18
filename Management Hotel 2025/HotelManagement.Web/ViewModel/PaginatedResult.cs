namespace Management_Hotel_2025.ViewModel
{
    public class PaginatedResult <T>
    {
        public List<T> Data { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PaginatedResult(List<T> data, int totalCount, int currentPage, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasPrevious = currentPage > 1;
            HasNext = currentPage < TotalPages;
        }

        public PaginatedResult()
        {
            Data = new List<T>();
            TotalCount = 0;
            CurrentPage = 1;
            PageSize = 10; // Default page size
            TotalPages = 0;
            HasPrevious = false;
            HasNext = false;
        }
    }
}
