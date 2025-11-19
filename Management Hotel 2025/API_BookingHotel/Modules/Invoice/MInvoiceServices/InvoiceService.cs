using API_BookingHotel.Modules.Invoice.InvoiceModels;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;

namespace API_BookingHotel.Modules.Invoice.MInvoiceServices
{
    public class InvoiceService : IInvoiceServices
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(ManagermentHotelContext dbcontext, ILogger<InvoiceService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        //lấy hóa đơn theo mã
        public async Task<InvoiceViewModel> GetInvoicePassengerByCode(string invoiceCode)
        {
            try
            {
                var InvoicesItem = await _dbcontext.Orders.Where(s => s.OrderCode == invoiceCode)
                    .Include(s => s.Booking)
                    .ThenInclude(s => s.BookingDetails)
                    .ThenInclude(s => s.Room)
                    .Select(s => new InvoiceViewModel()
                    {
                        InvoiceCode = s.OrderCode,
                        CustomerName = s.Booking.CustomerName,
                        RoomNumber = string.Join(", ", s.Booking.BookingDetails.Select(b => b.Room.RoomNumber)),
                        CheckInDate = s.Booking.RealTimeCheckIn,
                        CheckOutDate = s.Booking.RealTimeCheckOut,
                        TotalAmount = s.TotalAmount,
                        StatusInvoice = s.OrderStatus,
                        CreatedBy = "Phạm Trung Đức"
                    }).FirstOrDefaultAsync();


                return InvoicesItem ?? new InvoiceViewModel();

            }
            catch (Exception s)
            {
                _logger.LogInformation("Lỗi lấy danh sách hóa đơn: " + s.Message);
                return new InvoiceViewModel();
            }
        }

        // lấy danh sách toàn bộ hóa đơn
        public async Task<PagedResult<InvoiceViewModel>> GetListInvoicePasseners(string? SearchKey, DateTime? startdate, DateTime? enddate, int indexpage)
        {
            try
            {

                if (!startdate.HasValue) startdate = DateTime.Now.AddDays(-7);

                if (!enddate.HasValue) enddate = DateTime.Now;


                var query = _dbcontext.Orders
                        .Include(o => o.Booking)
                            .ThenInclude(b => b.BookingDetails)
                                .ThenInclude(d => d.Room)
                        .AsQueryable();

                if (startdate.HasValue)
                    query = query.Where(o => o.OrderDate >= startdate.Value);

                if (enddate.HasValue)
                    query = query.Where(o => o.OrderDate <= enddate.Value);


                if (!string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = SearchKey.Trim();

                    query = query.Where(o => o.CustomerName.Contains(SearchKey)  // where chỉ giữ lại các phần tử mà biểu thức trả về true
                                   || o.OrderCode.Contains(SearchKey));
                }

                int totalRecords = await query.CountAsync();

                query = query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((indexpage - 1) * 10)
                    .Take(10);


                var listInvoices = await query
                    .Select(o => new InvoiceViewModel
                    {
                        BookingCode = o.Booking.BookingCode,
                        InvoiceCode = o.OrderCode,
                        CustomerName = o.Booking.CustomerName,
                        RoomNumber = string.Join(", ", o.Booking.BookingDetails.Select(d => d.Room.RoomNumber)),
                        CheckInDate = o.Booking.RealTimeCheckIn,
                        CheckOutDate = o.Booking.RealTimeCheckOut,
                        TotalAmount = o.TotalAmount,
                        StatusInvoice = o.OrderStatus,
                        CreatedBy = "Phạm Trung Đức"
                    })
                    .ToListAsync();

                return new PagedResult<InvoiceViewModel>
                {
                    Items = listInvoices,
                    PageIndex = indexpage,
                    PageSize = 10,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)10),
                    StartTime = startdate ?? DateTime.Now,
                    EndTime = enddate ?? DateTime.Now

                };

            }
            catch (Exception s)
            {
                _logger.LogInformation("Lỗi lấy danh sách hóa đơn: " + s.Message);
                return new PagedResult<InvoiceViewModel>();
            }

        }

    }
}
