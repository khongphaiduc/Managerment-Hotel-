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



        [HttpGet]
        [Route("room/{id}")]      
        // Handles the EditRoom action.
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
        

        [HttpPut]
        [Route("room")]
        
        // Handles the EditRoom action.
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


        [HttpPost]
        [Route("room")]
       
        // Handles the CreateRoom action.
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
