using Management_Hotel_2025.Modules.WorkFile;
using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Mydata.Models;
using MyData.Models;
using System.Net.WebSockets;

namespace Management_Hotel_2025.Modules.Rooms.RoleAdmin.AdminServices
{
    public class AdminManagement : IAdminManagement
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IMyFiles _file;

        public AdminManagement(ManagermentHotelContext dbcontext, IMyFiles file)
        {
            _dbcontext = dbcontext;
            _file = file;
        }

        public async Task<bool> HideRoom(int idRoom)
        {
            if (idRoom <= 0)
            {
                return false;
            }
            var item = await _dbcontext.Rooms.Where(s => s.RoomId == idRoom).FirstOrDefaultAsync();

            if (item == null)
            {
                return false;
            }

            if (item.Status == "hidden")
            {
                item.Status = "Active";
            }
            else
            {
                item.Status = "hidden";
            }



            _dbcontext.Rooms.Update(item);
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        public AdJustRoom LoadTypeRoomAndAmentity()
        {
            AdJustRoom adJustRoom = new AdJustRoom();
            // load loại phòng 
            adJustRoom.AllRoomTypes = _dbcontext.RoomTypes
                .Select(rt => new RoomTypeViewModel
                {
                    RoomTypeId = rt.RoomTypeId,
                    TypeName = rt.Name
                }).ToList();
            // load tiện ích 
            adJustRoom.AllAvailableAmenities = _dbcontext.Amenities
                .Select(a => new AmenityViewModel
                {
                    Id = a.AmenityId,
                    Name = a.Name,

                }).ToList();
            return adJustRoom;
        }

        public List<int> NumberOfFloor()
        {

            var numberOfFloor = _dbcontext.Rooms.Select(r => r.Floor ?? 0).Distinct().ToList();

            return numberOfFloor;

        }

        public List<ViewRoomModel> SearchRoom(int? floor, string? status, string? key)
        {

            if (!string.IsNullOrEmpty(key))
            {
                return _dbcontext.Rooms.Where(s => s.RoomNumber.Contains(key)).Select(s => new ViewRoomModel()
                {

                    IdRoom = s.RoomId,
                    Name = s.RoomType.Name,
                    NumberOfRooms = s.RoomNumber,
                    Floor = s.Floor ?? 0,
                    Description = s.Description,
                    Price = s.RoomType.Price,
                    status = s.Status ?? "Hệ thông không nhận diện"

                }).ToList();

            }
            else
            {

                var item = _dbcontext.Rooms.Where(s => (!floor.HasValue || s.Floor == floor) &&
                                                        (string.IsNullOrEmpty(status) || s.Status == status))
                                                   .Select(r => new ViewRoomModel
                                                   {
                                                       IdRoom = r.RoomId,
                                                       Name = r.RoomType.Name,
                                                       NumberOfRooms = r.RoomNumber,
                                                       Floor = r.Floor ?? 0,
                                                       Description = r.Description,
                                                       Price = r.RoomType.Price,
                                                       status = r.Status ?? "Hệ thông không nhận diện"
                                                   }).ToList();

                return item;
            }
        }

        public List<string> StatusRoom()
        {
            var statusRooms = _dbcontext.Rooms.Select(r => r.Status ?? "Hệ thống không nhận diện").Distinct().ToList();
            return statusRooms;
        }

        public AdminListsViewRoom ViewListRoom()
        {
            // danh sách phòng khách nhận phòng hôm nay
            var item1 = _dbcontext.BookingDetails.Include(s => s.Room).ThenInclude(s => s.RoomType).Include(s => s.Booking)
                .Where(bd => bd.CheckInDate == DateTime.Now.Date)
                .Select(bd => new RoomTemporary
                {
                    NameCustomer = bd.Booking.CustomerName,
                    TypeRoom = bd.Room.RoomType.Name,
                    NumberOfRoom = int.Parse(bd.Room.RoomNumber),
                    DayCheckIn = (DateTime)bd.CheckInDate,
                    DayCheckOut = (DateTime)bd.CheckOutDate,
                    TypeCustomer = bd.Booking.TypePassenger
                }).ToList();



            // danh sách phòng khách trả phòng hôm nay
            var item2 = _dbcontext.BookingDetails.Include(s => s.Room).ThenInclude(s => s.RoomType).Include(s => s.Booking)
                .Where(bd => bd.CheckOutDate == DateTime.Now.Date)
                .Select(bd => new RoomTemporary
                {
                    NameCustomer = bd.Booking.CustomerName,
                    TypeRoom = bd.Room.RoomType.Name,
                    NumberOfRoom = int.Parse(bd.Room.RoomNumber),
                    DayCheckIn = (DateTime)bd.CheckInDate,
                    DayCheckOut = (DateTime)bd.CheckOutDate,
                    TypeCustomer = bd.Booking.TypePassenger
                }).ToList();



            // danh sách khách hàng đang lưu trú
            var item3 = _dbcontext.BookingDetails.Include(s => s.Room).ThenInclude(s => s.RoomType).Include(s => s.Booking)
                .Where(bd => bd.CheckInDate <= DateTime.Now.Date && bd.CheckOutDate >= DateTime.Now.Date && bd.Booking.Status == "CheckIn")
                .Select(bd => new RoomTemporary
                {
                    NameCustomer = bd.Booking.CustomerName,
                    TypeRoom = bd.Room.RoomType.Name,
                    NumberOfRoom = int.Parse(bd.Room.RoomNumber),
                    DayCheckIn = (DateTime)bd.CheckInDate,
                    DayCheckOut = (DateTime)bd.CheckOutDate,
                    TypeCustomer = bd.Booking.TypePassenger
                }).ToList();



            AdminListsViewRoom adminListsViewRoom = new AdminListsViewRoom()
            {
                ListCheckInToday = item1,
                ListCheckOutToday = item2,
                ListCustomerUsing = item3


            };

            return adminListsViewRoom;

        }

        // admim xem danh sách phòng
        public List<ViewRoomModel> ViewTypeRoom()
        {

            var items = _dbcontext.Rooms.Include(r => r.RoomType)
                .Select(r => new ViewRoomModel
                {
                    IdRoom = r.RoomId,
                    Name = r.RoomType.Name,
                    NumberOfRooms = r.RoomNumber,
                    Floor = r.Floor ?? 0,
                    Description = r.Description,
                    Price = r.RoomType.Price,
                    status = r.Status ?? "Hệ thông không nhận diện"
                }).ToList();
            return items;


        }
    }
}
