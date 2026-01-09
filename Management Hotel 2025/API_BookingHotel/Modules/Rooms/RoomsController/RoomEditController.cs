using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace API_BookingHotel.Modules.Rooms.RoomsController
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomEditController : ControllerBase
    {
        private readonly IEditableRoom _editroom;

        public RoomEditController(IEditableRoom editableRoom)
        {
            _editroom = editableRoom;
        }



        // lấy toàn bộ thông tin của 1 phòng
        [HttpGet]
        [Route("room/{id}")]      
        public async Task<IActionResult> EditRoom([FromRoute] int id)
        {
            string apihost = $"{Request.Scheme}://{Request.Host}";
            var room = await _editroom.GetFullInfoRoom(id, apihost);
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            else
            {
                return Ok(room);
            }
        }
        

        // chỉnh sửa thong tin của phòng 
        [HttpPut]
        [Route("room")]
        
        public async Task<IActionResult> EditRoom([FromForm] AdJustRoom room)
        {
            var result = await _editroom.EditRoomStatus(room);

            if (!result)
            {
                return BadRequest("Failed to edit room.");
            }
            else
            {
                return Ok("Change Room Successfully.");
            }
        }


        // tạo mới  phòng
        [HttpPost]
        [Route("room")]
       
        public async Task<IActionResult> CreateRoom([FromForm] AdJustRoom room)
        {
            var result = await _editroom.CreateNewRoom(room);
            if (!result)
            {
                return BadRequest("Failed to create room.");
            }
            else
            {
                return Ok("Create Room Successfully.");
            }
        }


    }
}
