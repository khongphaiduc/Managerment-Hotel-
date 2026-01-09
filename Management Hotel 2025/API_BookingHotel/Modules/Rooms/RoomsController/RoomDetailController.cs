
using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Mydata.Models;
using System.Collections.Concurrent;

namespace API_BookingHotel.Modules.Rooms.RoomsController
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class RoomDetailController : ControllerBase
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly RoomViewDetail _Mybooking;
        private readonly IDistributedCache _redisCache;
        private readonly ILogger<RoomDetailController> _ilogger;

        //  private static readonly SemaphoreSlim _roomDetailLock = new SemaphoreSlim(1, 1);

        private static ConcurrentDictionary<string, SemaphoreSlim> _LockSemaphore = new ConcurrentDictionary<string, SemaphoreSlim>();

        public RoomDetailController(ManagermentHotelContext dbcontext, RoomViewDetail MyBooings, IDistributedCache _RedisCache, ILogger<RoomDetailController> _logger)
        {
            _dbcontext = dbcontext;
            _Mybooking = MyBooings;
            _redisCache = _RedisCache;
            _ilogger = _logger;
        }

        [HttpGet("test")]
        public IActionResult Index()
        {
            return Accepted();
        }


        [HttpGet("room/{idRoom}")]
        public async Task<IActionResult> ViewDetaiRoom([FromRoute] string idRoom)
        {
            if (string.IsNullOrEmpty(idRoom))
            {
                return BadRequest("Room ID is required");
            }
            else
            {   // Lấy host + port của API đang chạy
                var apiHost = $"{Request.Scheme}://{Request.Host}";


                // check xem có trong Redis hay chưa 
                var room = await _redisCache.GetStringAsync($"RoomDetail_{idRoom}");

                if (room != null)
                {
                    // Nếu có trong Redis thì trả về 
                    var cachedRoom = System.Text.Json.JsonSerializer.Deserialize<ViewRoomDetail>(room);

                    _ilogger.LogInformation($"RoomDetail_{idRoom} Avaliable Redis");

                    return Ok(cachedRoom);
                }
                else
                {
                    // lấy semaphore của phòng 
                    var _roomDetailLock = _LockSemaphore.GetOrAdd(idRoom, new SemaphoreSlim(1, 1));

                    await _roomDetailLock.WaitAsync();

                    var rooms2 = await _redisCache.GetStringAsync($"RoomDetail_{idRoom}");

                    if (rooms2 != null)
                    {

                        var cachedRoom2 = System.Text.Json.JsonSerializer.Deserialize<ViewRoomDetail>(rooms2);
                        _ilogger.LogInformation($"RoomDetail_{idRoom} Avaliable Redis");
                        _roomDetailLock.Release();
                        return Ok(cachedRoom2);
                    }

                    try
                    {
                        var IDRoomAfterCheck = int.Parse(idRoom);
                        var result = await _Mybooking.ViewDetailRoomAsync(IDRoomAfterCheck, apiHost);

                        if (result != null)
                        {
                            var serializedRoom = System.Text.Json.JsonSerializer.Serialize(result);
                            var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                            await _redisCache.SetStringAsync($"RoomDetail_{idRoom}", serializedRoom, options);

                            return Ok(result);
                        }
                        else
                        {
                            return NotFound("Room not found");
                        }
                    }
                    finally
                    {
                        _roomDetailLock.Release(); // giải phóng semaphore
                    }

                }
            }
        }
    }
}


