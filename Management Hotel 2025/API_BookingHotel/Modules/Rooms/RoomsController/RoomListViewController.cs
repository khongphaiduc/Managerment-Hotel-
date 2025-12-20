
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
using API_BookingHotel.Modules.Rooms.DTOs;
namespace API_BookingHotel.Modules.Rooms.RoomsController
{
    [Route("api/roomtotel")]
    [ApiController]
    public class RoomListViewController : ControllerBase
    {
        private readonly IRoomService _IRoomService;
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IMemoryCache _iMemoryCatch;
        private readonly ILogger<RoomListViewController> _logger;
        private readonly IDistributedCache _redisCache;

        public RoomListViewController(IRoomService _RoomService, ManagermentHotelContext dbcontext, IMemoryCache memoryCache, ILogger<RoomListViewController> logger, IDistributedCache redisCache)
        {
            _IRoomService = _RoomService;
            _dbcontext = dbcontext;
            _iMemoryCatch = memoryCache;
            _logger = logger;
            _redisCache = redisCache;
        }


        // lấy danh sách phòng 
        [AllowAnonymous]
        [HttpGet("room")]
        public async Task<IActionResult> SearchRoomAdvanceVersion2([FromQuery] RoomFilterRequest roomRequest)
        {
            string apihost = $"{Request.Scheme}://{Request.Host}";

            // nếu user không chọn ngày thì mặc định tính từ hôm nay tới 7 ngày tiếp theo
            if (roomRequest.StartDate == null)
                roomRequest.StartDate = DateTime.Now.ToString();
            if (roomRequest.EndDate == null)
                roomRequest.EndDate = DateTime.Now.AddDays(7).ToString();

            DateTime newCheckIn = DateTime.Parse(roomRequest.StartDate);
            DateTime newCheckOut = DateTime.Parse(roomRequest.EndDate);

            // lấy db trước khi skip
            int TotalItems = await _dbcontext.Rooms
                .Include(s => s.RoomType)
                .Include(s => s.BookingDetails)
                .Where(s => (s.Status == "Active") &&
                            (!roomRequest.Floor.HasValue || s.Floor == roomRequest.Floor.Value) &&
                            (!roomRequest.PriceMin.HasValue || s.RoomType.Price >= roomRequest.PriceMin.Value) &&
                            (!roomRequest.PriceMax.HasValue || s.RoomType.Price <= roomRequest.PriceMax.Value) &&
                            (!roomRequest.Person.HasValue || s.RoomType.MaxGuests == roomRequest.Person.Value) &&
                            !s.BookingDetails.Any(bd =>
                                bd.Booking.Status != "Cancelled" &&
                                newCheckIn < bd.CheckOutDate &&
                                newCheckOut > bd.CheckInDate))
                .CountAsync();

            // Redis cache
            string cacheKey = $"SearchRoom_{roomRequest.PageCurrent}_{roomRequest.NumerItemOfPage}_{roomRequest.Floor}_{roomRequest.PriceMin}_{roomRequest.PriceMax}_{roomRequest.Person}_{roomRequest.StartDate}_{roomRequest.EndDate}";

            List<ViewRoom> ListResult = null;

            // kiểm tra cache Redis
            var cachedData = await _redisCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {

                ListResult = JsonSerializer.Deserialize<List<ViewRoom>>(cachedData);
            }
            else
            {

                ListResult = await _IRoomService.SearchRoomByAdvance(roomRequest,apihost);

                // serialize và lưu vào Redis
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))   // nếu 5 phút không truy cập, tự xóa
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));  // thời gian tối đa

                await _redisCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ListResult), cacheOptions);
            }

            return Ok(new PaginationResult<ViewRoom>(ListResult, TotalItems, roomRequest.PageCurrent, roomRequest.NumerItemOfPage, newCheckIn, newCheckOut));
        }
    }
}
