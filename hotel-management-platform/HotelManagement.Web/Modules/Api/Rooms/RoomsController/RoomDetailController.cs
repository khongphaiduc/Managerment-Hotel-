using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly RoomViewDetail _myBooking;
        private readonly IDistributedCache _redisCache;
        private readonly ILogger<RoomDetailController> _logger;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> LockSemaphore = new();

        public RoomDetailController(RoomViewDetail myBooking, IDistributedCache redisCache, ILogger<RoomDetailController> logger)
        {
            _myBooking = myBooking;
            _redisCache = redisCache;
            _logger = logger;
        }

        [HttpGet("test")]
// Provides a lightweight health/test endpoint for the room-detail API.
        public IActionResult Index() => Accepted();

        [HttpGet("room/{idRoom}")]
// Returns room details, using Redis cache when a cached representation is available.
        public async Task<IActionResult> ViewDetaiRoom([FromRoute] string idRoom)
        {
            if (string.IsNullOrEmpty(idRoom))
            {
                return BadRequest("Room ID is required");
            }

            var apiHost = $"{Request.Scheme}://{Request.Host}";
            var cacheKey = $"RoomDetail_{idRoom}";
            var cached = await _redisCache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Room detail {RoomId} available in Redis", idRoom);
                return Ok(System.Text.Json.JsonSerializer.Deserialize<ViewRoomDetail>(cached));
            }

            var roomLock = LockSemaphore.GetOrAdd(idRoom, _ => new SemaphoreSlim(1, 1));
            await roomLock.WaitAsync();
            try
            {
                cached = await _redisCache.GetStringAsync(cacheKey);
                if (cached != null)
                {
                    return Ok(System.Text.Json.JsonSerializer.Deserialize<ViewRoomDetail>(cached));
                }

                var result = await _myBooking.ViewDetailRoomAsync(int.Parse(idRoom), apiHost);
                if (result == null)
                {
                    return NotFound("Room not found");
                }

                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
                await _redisCache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(result), options);
                return Ok(result);
            }
            finally
            {
                roomLock.Release();
            }
        }
    }
}
