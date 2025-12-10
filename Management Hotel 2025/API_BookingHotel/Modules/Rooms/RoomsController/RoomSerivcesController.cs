
using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Mydata.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace API_BookingHotel.Modules.Rooms.RoomsController
{
    [Route("api/roomtotel")]
    [ApiController]
    public class RoomSerivcesController : ControllerBase
    {
        private readonly IRoomService _IRoomService;
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IMemoryCache _iMemoryCatch;
        private readonly ILogger<RoomSerivcesController> _logger;
        private readonly IDistributedCache _redisCache;

        public RoomSerivcesController(IRoomService _RoomService, ManagermentHotelContext dbcontext, IMemoryCache memoryCache, ILogger<RoomSerivcesController> logger, IDistributedCache redisCache)
        {
            _IRoomService = _RoomService;
            _dbcontext = dbcontext;
            _iMemoryCatch = memoryCache;
            _logger = logger;
            _redisCache = redisCache;
        }

        // api cung cấp  danh sách room cho khách hàng 
        [AllowAnonymous]
        [HttpGet("room")]
        public async Task<IActionResult> SearchRoomAdvance(int PageCurrent, int NumerItemOfPage, int? Floor, int? PriceMin, int? PriceMax, int? Person, string? StartDate, string? EndDate)
        {
            string apihost = $"{Request.Scheme}://{Request.Host}";

            // nếu user không chọn ngày thì mặc định tính từ hôm nay tới 7 ngày tiếp theo
            if (StartDate == null)
                StartDate = DateTime.Now.ToString();
            if (EndDate == null)
                EndDate = DateTime.Now.AddDays(7).ToString();

            DateTime newCheckIn = DateTime.Parse(StartDate);
            DateTime newCheckOut = DateTime.Parse(EndDate);

            // lấy db trước khi skip
            int TotalItems = await _dbcontext.Rooms
                .Include(s => s.RoomType)
                .Include(s => s.BookingDetails)
                .Where(s => (s.Status == "Active") &&
                            (!Floor.HasValue || s.Floor == Floor.Value) &&
                            (!PriceMin.HasValue || s.RoomType.Price >= PriceMin.Value) &&
                            (!PriceMax.HasValue || s.RoomType.Price <= PriceMax.Value) &&
                            (!Person.HasValue || s.RoomType.MaxGuests == Person.Value) &&
                            !s.BookingDetails.Any(bd =>
                                bd.Booking.Status != "Cancelled" &&
                                newCheckIn < bd.CheckOutDate &&
                                newCheckOut > bd.CheckInDate))
                .CountAsync();

            // Redis cache
            string cacheKey = $"SearchRoom_{PageCurrent}_{NumerItemOfPage}_{Floor}_{PriceMin}_{PriceMax}_{Person}_{StartDate}_{EndDate}";

            List<ViewRoom> ListResult = null;

            // kiểm tra cache Redis
            var cachedData = await _redisCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // deserialize từ JSON
                ListResult = JsonSerializer.Deserialize<List<ViewRoom>>(cachedData);
            }
            else
            {

                ListResult = await _IRoomService.SearchRoomByAdvance(PageCurrent, NumerItemOfPage, Floor, PriceMin, PriceMax, Person, StartDate, EndDate, apihost);

                // serialize và lưu vào Redis
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))   // nếu 5 phút không truy cập, tự xóa
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));  // thời gian tối đa

                await _redisCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ListResult), cacheOptions);
            }

            return Ok(new PaginationResult<ViewRoom>(ListResult, TotalItems, PageCurrent, NumerItemOfPage, newCheckIn, newCheckOut));
        }



        // lấy danh sách phòng cho thằng management
        [AllowAnonymous]
        [HttpGet("rooms")]
        public async Task<IActionResult> GetListRoomForManagement(string option, int PageCurrent, int NumerItemOfPage, int? Floor, int? PriceMin, int? PriceMax, int? Person, string? StartDate, string? EndDate)
        {
            // nếu user không chọn ngày thì mặc định tính từ ngay hôm nay tói 7 ngày tiếp theo 
            if (StartDate == null)
            {
                StartDate = DateTime.Now.ToString();
            }
            if (EndDate == null)
            {
                DateTime Today = DateTime.Now;
                EndDate = Today.AddDays(7).ToString();
            }

            DateTime newCheckIn = DateTime.Parse(StartDate);
            DateTime newCheckOut = DateTime.Parse(EndDate);

            // lấy db trước khi mà skip
            int TotalItems = await _dbcontext.Rooms
                         .Include(s => s.RoomType).Include(s => s.BookingDetails)
                         .Where(s => (!Floor.HasValue || s.Floor == Floor.Value) &&
                               (!PriceMin.HasValue || s.RoomType.Price >= PriceMin.Value) &&
                               (!PriceMax.HasValue || s.RoomType.Price <= PriceMax.Value) &&
                               (!Person.HasValue || s.RoomType.MaxGuests == Person.Value)
                                                      )

                         .CountAsync();

            var ListResult = await _IRoomService.SearchRoomByAdvanceForManagement(option, PageCurrent, NumerItemOfPage, Floor, PriceMin, PriceMax, Person, StartDate, EndDate);

            return Ok(new PaginationResult<ViewRoom>(ListResult, TotalItems, PageCurrent, NumerItemOfPage, newCheckIn, newCheckOut));  //  lưu vào construcor của PaginationResult để trả về
        }

    }
}
