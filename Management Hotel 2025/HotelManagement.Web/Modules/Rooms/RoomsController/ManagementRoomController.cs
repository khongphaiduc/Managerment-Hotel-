using API_BookingHotel.Modules.Rooms.DTOs;
using API_BookingHotel.Modules.Rooms.RoomsService;
using API_BookingHotel.ViewModels;
using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;

namespace Management_Hotel_2025.Modules.Rooms.RoomsController
{
    [Route("room")]
    public class ManagementRoomController : Controller
    {
        private readonly ManagermentHotelContext _dbContext;
        private readonly API_BookingHotel.Modules.Rooms.RoomsService.IRoomService _roomService;
        private readonly RoomViewDetail _roomDetailService;

        public ManagementRoomController(
            ManagermentHotelContext dbContext,
            API_BookingHotel.Modules.Rooms.RoomsService.IRoomService roomService,
            RoomViewDetail roomDetailService)
        {
            _dbContext = dbContext;
            _roomService = roomService;
            _roomDetailService = roomDetailService;
        }

        [AllowAnonymous]
        [Route("list")]
        [Route("list/{PageCurrent:int}/{NumerItemOfPage:int}")]
        // Displays the filtered room list.
        public async Task<IActionResult> ViewListRoomVer2(RoomFilterRequest roomRequest)
        {
            roomRequest.PageCurrent = roomRequest.PageCurrent < 1 ? 1 : roomRequest.PageCurrent;
            roomRequest.NumerItemOfPage = roomRequest.NumerItemOfPage < 1 ? 8 : roomRequest.NumerItemOfPage;
            roomRequest.StartDate ??= DateTime.Now.ToString("yyyy-MM-dd");
            roomRequest.EndDate ??= DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

            ViewBag.Floor = roomRequest.Floor;
            ViewBag.PageCurrent = roomRequest.PageCurrent;
            ViewBag.NumerItemOfPage = roomRequest.NumerItemOfPage;
            ViewBag.PriceMin = roomRequest.PriceMin;
            ViewBag.PriceMax = roomRequest.PriceMax;
            ViewBag.Person = roomRequest.Person;
            ViewBag.StartDate = roomRequest.StartDate;
            ViewBag.EndDate = roomRequest.EndDate;

            HttpContext.Session.SetString("StartDate", roomRequest.StartDate);
            HttpContext.Session.SetString("EndDate", roomRequest.EndDate);

            var apiHost = $"{Request.Scheme}://{Request.Host}";
            var rooms = await _roomService.SearchRoomByAdvance(roomRequest, apiHost);
            var totalCount = await _roomService.CountRoomByAdvance(roomRequest);
            var model = new PaginatedResult<ViewRoomModel>(
                rooms.Select(room => new ViewRoomModel
                {
                    IdRoom = room.IdRoom,
                    Name = room.Name,
                    NumberOfRooms = room.NumberOfRooms,
                    Floor = room.Floor,
                    Description = room.Description,
                    Image = room.Image,
                    Price = room.Price
                }).ToList(),
                totalCount,
                roomRequest.PageCurrent,
                roomRequest.NumerItemOfPage);

            return View(model);
        }

        [AllowAnonymous]
        [Route("detail")]
        // Displays the details of a selected room.
        public async Task<IActionResult> ViewDetailRoomVer2([FromQuery] int idRoom)
        {
            var apiHost = $"{Request.Scheme}://{Request.Host}";
            var room = await _roomDetailService.ViewDetailRoomAsync(idRoom, apiHost);
            if (room == null)
            {
                return NotFound();
            }

            return View(new Management_Hotel_2025.ViewModel.ViewDetailRoom
            {
                RoomId = room.RoomId,
                RoomTypeId = room.RoomTypeId,
                NameType = room.NameType,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                Status = room.Status,
                Description = room.Description,
                PathImage = room.PathImage,
                Price = room.Price,
                MaxGuests = room.MaxGuests,
                ListPathImage = room.ListPathImage.ToList(),
                ListAmenites = room.ListAmenites
                    .Select(a => new MyData.Models.Amenity
                    {
                        AmenityId = a.AmenityId,
                        Name = a.Name,
                        Description = a.Description,
                        UrlImage = a.UrlImage
                    }).ToList()
            });
        }

        [Route("date")]
        // Displays the room search date form.
        public IActionResult ChosseDate() => View();
    }
}
