namespace Management_Hotel_2025.Modules.Invoices.InvocieModels
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }  // danh sách hóa đơn
        public int PageIndex { get; set; }        // trang hiện tại
        public int PageSize { get; set; }         // số bản ghi mỗi trang
        public int TotalPages { get; set; }       // tổng số trang
        public int TotalRecords { get; set; }     // tổng số bản ghi


        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
