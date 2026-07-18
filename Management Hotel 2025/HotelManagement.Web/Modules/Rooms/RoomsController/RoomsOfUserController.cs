using Management_Hotel_2025.Modules.Rooms.RoomService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.Rooms.RoomsController
{
    public class RoomsOfUserController : Controller
    {
        private readonly IRoomService _IRoomService;

        public RoomsOfUserController(IRoomService roomService)
        {
            _IRoomService = roomService;

        }

        [Authorize(Roles ="User")]
        public IActionResult ViewListRooms()
        {
            int IdUser = User.FindFirst("IdUser") != null ? int.Parse(User.FindFirst("IdUser").Value) : 0;

            var ListRooms = _IRoomService.GetListRoomOfUser(IdUser);
    
            return View(ListRooms);
        }


    }
}
