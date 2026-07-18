
using Management_Hotel_2025.Modules.Rooms.ManagementRoom;
using Management_Hotel_2025.Modules.Rooms.RoomService;
using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Mydata.Models;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;


namespace Management_Hotel_2025.Modules.Rooms.RoomsController
{

    public class StaffManagementRoomController : Controller
    {
        private readonly IManagementRoom _IManagementRoom;
        private readonly IManagementBooking _IManagementBooking;
        private readonly IReceptionService _IreceptionService;
        private readonly ManagermentHotelContext _dbcontext;
        private readonly ILogger<StaffManagementRoomController> _logger;
        private readonly IOrder _order;

        public List<Passengers> PassengersList { get; set; } = new List<Passengers>();
        public StaffManagementRoomController(IManagementRoom managementRoom, IManagementBooking managementBooking, IReceptionService receptionService, ManagermentHotelContext dbcontext, ILogger<StaffManagementRoomController> logger, IOrder order)
        {
            _IManagementRoom = managementRoom;
            _IManagementBooking = managementBooking;
            _IreceptionService = receptionService;
            _dbcontext = dbcontext;
            _logger = logger;
            _order = order;
        }


        [Authorize(Roles = "Staff,Admin")]
        [HttpGet]
        public async Task<IActionResult> StaffViewListRoom(string option, int? Floor, DateTime StartDate, DateTime EndDate)
        {
            if (StartDate == DateTime.MinValue || EndDate == DateTime.MinValue)
            {
                var Today = DateTime.Now;
                StartDate = Today;
                EndDate = Today.AddDays(7);
            }

            if (option == null)
            {
                option = "all";
            }



            ViewBag.option = option;
            ViewBag.Floor = Floor;
            ViewBag.StartDate = StartDate.ToString("yyyy-MM-dd"); // format cho input date
            ViewBag.EndDate = EndDate.ToString("yyyy-MM-dd");

            var ListRoom = await _IManagementRoom.FilterRoom(option, Floor, StartDate, EndDate);

            return View(ListRoom);
        }

        // search room
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet]
        public async Task<IActionResult> StaffSearchByIdRoom(string IdRoom)
        {
            if (string.IsNullOrEmpty(IdRoom))
            {
                return RedirectToAction("StaffViewListRoom");
            }
            else
            {
                List<ViewRoomModel> list = new List<ViewRoomModel>();
                list.Add(await _IManagementRoom.FilterByIdRoom(IdRoom));
                return View("StaffViewListRoom", list);
            }

        }


        public async Task<IActionResult> StaffViewDetailRoom(string IdRoom)
        {
            var Room = await _IManagementRoom.FilterByIdRoom(IdRoom);
            return View(Room);
        }




        public IActionResult StaffBoookingRoom()
        {
            return View();
        }



        public IActionResult ViewCalenderBookingOfRoom(int IdRoom, string NumberRoom)
        {
            ViewBag.NumberRoom = NumberRoom;
            var calender = _IManagementRoom.GetListDateBookingOfRoom(IdRoom);

            return View(calender);
        }


        public IActionResult ViewMapOfRoom()
        {
            return View(_IManagementRoom.getListMapRoomToDay());
        }







        // check in 
        public IActionResult CheckInPassengers(string bookingcode)
        {
            var booking = _IreceptionService.CheckIn(bookingcode);
            return View(booking);
        }


        [HttpPost]
        public IActionResult CheckInPassengers([FromBody] Booking booking)
        {
            var item = _IreceptionService.CheckIn(booking);

            if (item)
            {
                return Json(new { success = true, message = "Check-in successful!" });
            }
            else
            {
                return Json(new { success = false, message = "Check-in failed. Please try again." });
            }

        }


        [HttpGet]
        public IActionResult RegisterGuestInfo(string bookingcode)
        {
            return View();
        }



        [HttpPost]
        public IActionResult RegisterGuestInfo([FromBody] List<PassengerDto> passengers)
        {

            // booking  code 
            string bookingCode = passengers.First().BookingCode;



            if (passengers == null || passengers.Count == 0)
            {
                return Json(new { success = false, message = "Danh sách trống hoặc dữ liệu không hợp lệ!" });
            }

            _logger.LogInformation($"Số lượng item của khách là :{passengers.Count}");

            foreach (var p in passengers)
            {

                var room = p.RoomNumber;

                var idRoom = _dbcontext.Rooms.Where(r => r.RoomNumber == room).Select(r => r.RoomId).FirstOrDefault();

                var idBookingdetail = _dbcontext.BookingDetails
                    .Where(bd => bd.Booking.BookingCode == bookingCode && bd.RoomId == idRoom)
                    .Select(bd => bd.BookingDetailId)
                    .FirstOrDefault();



                _dbcontext.Guests.Add(new Guests()
                {
                    CodePersonal = p.IdNumber,
                    FullName = p.FullName,
                    Gender = p.Sex,
                    PhoneNumber = p.Phone,
                    Nationality = p.Nationality,
                    Note = p.Note,
                    BookingDetailId = idBookingdetail,
                    Address = "Vietnam"
                });


            }
            return _dbcontext.SaveChanges() > 0 ? Json(new { success = true, message = $"{passengers.Count}" }) : Json(new { success = false, message = "✅ Lưu danh sách thất baik!" });
        }



        [HttpGet]
        public IActionResult GetAvailableRooms(string bookingcode)
        {

            var room = _dbcontext.Bookings.Include(s => s.BookingDetails).ThenInclude(s => s.Room).Where(s => s.BookingCode == bookingcode).Select(s => new
            {
                id = s.BookingDetails.FirstOrDefault().RoomId,
                name = s.BookingDetails.Select(bd => bd.Room.RoomNumber)

            });
            return Json(room);
        }


        [HttpGet]
        public async Task<IActionResult> CheckOutPassenger(string bookingcode)
        {

            var order = await _order.ViewOrder(bookingcode);
            ViewBag.TimeCheckOut = DateTime.Now;
            return View(order);
        }

        // confirm check-out
        [HttpPut]
        public async Task<IActionResult> ConfirmCheckOutPassenger([FromBody] Order orderpassager)
        {
            var idStaff = int.Parse(User.FindFirst("IdUser").Value);

            var result = await _order.ConfirmCheckOut(orderpassager, "Cash", idStaff);

            if (result)
            {
                return Json(new { success = true, message = "Check-out successful!" });
            }
            else
            {
                return Json(new { success = false, message = "Check-out failed. Please try again." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmCheckOutPassengerQR([FromBody] Order orderpassager)
        {
            var idStaff = int.Parse(User.FindFirst("IdUser").Value);

            var result = await _order.ConfirmCheckOut(orderpassager, "QR Code", idStaff);

            if (result)
            {
                return Json(new { success = true, message = "Create order thành công " });
            }
            else
            {
                return Json(new { success = false, message = "Tạo order thất bại ,vui lòng kiểm tra lại!" });
            }
        }


        public IActionResult BookingsView(string search, DateTime? DateStart, DateTime? EndDate)
        {
            DateTime start = DateStart ?? DateTime.Now.AddMonths(-1);
            DateTime end = EndDate ?? DateTime.Now.AddMonths(1);

            ViewBag.DateStart = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");

            List<BookingItem> list;

            if (!string.IsNullOrEmpty(search))
            {
                list = _IManagementBooking.GetListBooking(search);
            }
            else
            {
                list = _IManagementBooking.GetListBooking(start, end);
            }

            return View(list);
        }

        // xem detail booking
        public IActionResult ViewDetailBooking(string Code)
        {
            var detailbooking = _IManagementBooking.ViewDetailBooking(Code);

            return View(detailbooking);
        }



        [HttpGet]
        public IActionResult RoomViewPassengers(int idbookingdetail)
        {
            return View(_IManagementRoom.ViewDetailRoomPassengers(idbookingdetail));
        }
    }
}
