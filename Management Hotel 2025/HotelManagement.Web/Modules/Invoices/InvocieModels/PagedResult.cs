namespace Management_Hotel_2025.Modules.Invoices.InvocieModels
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }


        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
