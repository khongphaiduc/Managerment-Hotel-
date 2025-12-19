
using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;

namespace API_BookingHotel.Modules.Rooms.RoomsController
{
    [Route("api")]
    [ApiController]
    public class BookingHotelController : ControllerBase
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly RoomViewDetail _Mybooking;

        public BookingHotelController(ManagermentHotelContext dbcontext, RoomViewDetail MyBooings)
        {
            _dbcontext = dbcontext;
            _Mybooking = MyBooings;
        }

        // Allow user to view detail the room
        [AllowAnonymous]
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

                var IDRoomAfterCheck = int.Parse(idRoom);
                var result = await _Mybooking.ViewDetailRoomAsync(IDRoomAfterCheck,apiHost);
                if (result != null)
                {
                    return Ok(result); // Trong WEB API của ASP.NET thì các model chuyền qua OK(object) sẽ tự động chuyển thành JSON
                }
                else
                {
                    return BadRequest("Room ID is required");
                }
            }
        }

    }
}
