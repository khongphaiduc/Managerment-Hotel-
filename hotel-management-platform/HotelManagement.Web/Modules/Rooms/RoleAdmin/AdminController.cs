using Management_Hotel_2025.Modules.Rooms.RoleAdmin.AdminServices;
using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.Rooms.RoleAdmin
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IAdminManagement _adminManagement;
        private readonly API_BookingHotel.Modules.Rooms.RoomsService.IEditableRoom _editableRoom;

        public AdminController(
            IAdminManagement adminManagement,
            API_BookingHotel.Modules.Rooms.RoomsService.IEditableRoom editableRoom)
        {
            _adminManagement = adminManagement;
            _editableRoom = editableRoom;
        }

        [AllowAnonymous]
        [HttpPut("hide/{idroom}")]
        // Hides or activates a room.
        public async Task<IActionResult> HideRoom(int idroom)
        {
            var result = await _adminManagement.HideRoom(idroom);
            return result
                ? Ok(new { success = true, message = "Room status updated successfully." })
                : BadRequest(new { success = false, message = "Unable to update room status." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("room")]
        // Displays the admin room list.
        public IActionResult AdminManagementRoom(int? floor, string? status, string? key)
        {
            key = string.IsNullOrWhiteSpace(key) ? key : key.Trim();
            ViewBag.floor = floor;
            ViewBag.status = status;
            ViewBag.key = key;

            var rooms = !floor.HasValue && string.IsNullOrEmpty(status) && string.IsNullOrEmpty(key)
                ? _adminManagement.ViewTypeRoom()
                : _adminManagement.SearchRoom(floor, status, key);

            return View(new AdminManagementRoom
            {
                ListFloor = _adminManagement.NumberOfFloor(),
                ListStatusRoom = _adminManagement.StatusRoom(),
                ListViewRooms = rooms
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("serveralroom")]
        // Displays the admin dashboard room summary.
        public IActionResult AdminHomePage() => View(_adminManagement.ViewListRoom());

        [Authorize(Roles = "Admin")]
        [HttpGet("room/{idRoom}")]
        // Displays the room editing form.
        public async Task<IActionResult> AdjustRoom(int idRoom)
        {
            var apiHost = $"{Request.Scheme}://{Request.Host}";
            var room = await _editableRoom.GetFullInfoRoom(idRoom, apiHost);
            return room == null ? NotFound() : View(ToMvcModel(room));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("room/{idRoom}")]
        // Saves room changes.
        public async Task<IActionResult> AdjustRoom(AdJustRoom room)
        {
            var result = await _editableRoom.EditRoomStatus(ToApiModel(room));
            return result
                ? Ok(new { success = true, message = "Room updated successfully." })
                : BadRequest(new { success = false, message = "Unable to update room." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("rooms")]
        // Displays the create-room form.
        public IActionResult CreateNewRoom() => View(_adminManagement.LoadTypeRoomAndAmentity());

        [Authorize(Roles = "Admin")]
        [HttpPost("rooms")]
        // Creates a new room.
        public async Task<IActionResult> CreateRoom(AdJustRoom room)
        {
            var result = await _editableRoom.CreateNewRoom(ToApiModel(room));
            return result
                ? Ok(new { success = true, message = "Room created successfully." })
                : BadRequest(new { success = false, message = "Unable to create room." });
        }

        private static API_BookingHotel.ViewModels.AdJustRoom ToApiModel(AdJustRoom room) => new()
        {
            RoomId = room.RoomId,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            PricePerNight = room.PricePerNight,
            Description = room.Description,
            DeletedAmenity = room.DeletedAmenity,
            NewAmenities = room.NewAmenities,
            DeletedImageIds = room.DeletedImageIds,
            NewImages = room.NewImages,
            AvatarRoom = room.AvatarRoom
        };

        private static AdJustRoom ToMvcModel(API_BookingHotel.ViewModels.AdJustRoom room) => new()
        {
            RoomId = room.RoomId,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            PricePerNight = room.PricePerNight,
            Description = room.Description,
            AllRoomTypes = room.AllRoomTypes.Select(type => new RoomTypeViewModel
            {
                RoomTypeId = type.RoomTypeId,
                TypeName = type.TypeName
            }).ToList(),
            AllAvailableAmenities = room.AllAvailableAmenities.Select(amenity => new AmenityViewModel
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Icon = amenity.Icon
            }).ToList(),
            CurrentImages = room.CurrentImages.Select(image => new ImageViewModel
            {
                Id = image.Id,
                Url = image.Url
            }).ToList(),
            AvatarRoomRecive = room.AvatarRoomRecive
        };
    }
}
