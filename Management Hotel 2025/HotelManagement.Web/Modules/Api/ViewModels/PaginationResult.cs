namespace API_BookingHotel.ViewModels
{
    public class PaginationResult <T>
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
        public PaginationResult(List<T> data, int totalItem, int currentPage, int pageSize, DateTime startDate, DateTime endDate)
        {
            Data = data;
            TotalCount = totalItem;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalItem / pageSize);
            HasPrevious = currentPage > 1;
            HasNext = currentPage < TotalPages;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
