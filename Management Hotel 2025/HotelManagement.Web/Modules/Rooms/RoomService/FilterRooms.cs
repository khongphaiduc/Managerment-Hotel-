
using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public class FilterRooms : IManagementRoom
    {
        private readonly ManagermentHotelContext _Dbcontext;
        private const string ImagePath = "/AvatarImages";
        public FilterRooms(ManagermentHotelContext dbcontext)
        {

            _Dbcontext = dbcontext;
        }

        public void CreateRoom()
        {
            throw new NotImplementedException();
        }

        public void DeleteRoom()
        {
            throw new NotImplementedException();
        }

        public async Task<ViewRoomModel> FilterByIdRoom(string IdRoom)
        {
            return await _Dbcontext.Rooms.Include(s => s.RoomType).Where(s => s.RoomNumber.Equals(IdRoom)).Select(s => new ViewRoomModel()
            {
                IdRoom = s.RoomId,
                Name = s.RoomType.Name,
                Floor = (int)s.Floor,
                Description = s.Description,
                Image = s.PathImage,
                Price = s.RoomType.Price,
                NumberOfRooms = s.RoomNumber
            }).FirstOrDefaultAsync() ?? new ViewRoomModel();

        }


        public async Task<List<ViewRoomModel>> FilterRoom(string Option, int? Floor, DateTime startdate, DateTime enddate)
        {
            var list = await _Dbcontext.Rooms
                .Include(s => s.RoomType)
                .Include(s => s.BookingDetails)
                    .ThenInclude(s => s.Booking)
                .Where(s =>
                    (!Floor.HasValue || s.Floor == Floor) &&
                    (
                         Option == "all" ||
                        (Option == "Success" && s.BookingDetails.Any(bd => bd.Booking.Status == "Success" && startdate < bd.CheckOutDate && enddate > bd.CheckInDate)) ||
                        (Option == "Available" && !s.BookingDetails.Any(bd => bd.Booking.Status != "Cancelled" && startdate < bd.CheckOutDate && enddate > bd.CheckInDate)) ||
                        (Option == "Maintenance" && s.Status.Equals("Maintenance")) ||
                        (Option == "CheckIn" && s.BookingDetails.Any(s => s.Booking.Status == "Checkin"))
                    )
                )
                .Select(room => new ViewRoomModel()
                {
                    IdRoom = room.RoomId,
                    Name = room.RoomType.Name,
                    Floor = room.Floor ?? 1,
                    Description = room.Description,
                    Image = room.PathImage.StartsWith("http") ? room.PathImage : $"{ImagePath}/{room.PathImage}",
                    Price = room.RoomType.Price,
                    NumberOfRooms = room.RoomNumber,

                })
                .ToListAsync();

            return list;
        }

        public void UpdateRoom()
        {
            throw new NotImplementedException();
        }

        public void ViewDetailRoom()
        {
            throw new NotImplementedException();
        }


        public List<BookingInfo> GetListDateBookingOfRoom(int IdRoom)
        {

            var list = _Dbcontext.BookingDetails
                 .Include(s => s.Booking)
                 .Where(s => s.RoomId == IdRoom && s.Booking.Status != "Cancelled")
                 .Select(s => new BookingInfo()
                 {
                     StartDate = s.CheckInDate.Value,
                     EndDate = s.CheckOutDate.Value,
                     CustomerName = s.Booking.CustomerName,
                     Status = s.Booking.Status,

                 }).ToList();
            return list;
        }

        public void ViewDetailRoom(int idRoom)
        {
            throw new NotImplementedException();
        }

        public List<MapRoom> getListMapRoomToDay()
        {
            DateTime today = DateTime.Now.Date;

            var list = _Dbcontext.Rooms
                .Include(r => r.BookingDetails)
                    .ThenInclude(bd => bd.Booking)
                .Include(r => r.RoomType)
                .AsEnumerable()
                .Select(r =>
                {
                    var todayBooking = r.BookingDetails
                        .FirstOrDefault(bd => bd.CheckInDate <= today && bd.CheckOutDate >= today);

                    string status;

                    if (r.BookingDetails == null || !r.BookingDetails.Any())
                    {
                        status = "available";
                    }
                    else if (todayBooking != null)
                    {
                        string bookingStatus = todayBooking.Booking.Status;

                        if (bookingStatus.Equals("CheckIn", StringComparison.OrdinalIgnoreCase))
                        {
                            status = "occupied";
                        }
                        else if (bookingStatus.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        {
                            status = "reserved";
                        }
                        else if (bookingStatus.Equals("overdue", StringComparison.OrdinalIgnoreCase))
                        {
                            status = "overdue";
                        }
                        else if (bookingStatus.Equals("blocked", StringComparison.OrdinalIgnoreCase))
                        {
                            status = "blocked";
                        }
                        else
                        {
                            status = "available";
                        }
                    }
                    else
                    {
                        status = "available";
                    }

                    return new MapRoom
                    {
                        IdRoom = r.RoomId,
                        number = r.RoomNumber,
                        type = r.RoomType?.Name ?? "Không rõ",
                        status = status,
                        idBookingDetail = todayBooking?.BookingDetailId
                    };
                })
                .ToList();

            return list;
        }


        public RoomPassengers ViewDetailRoomPassengers(int idbookingdetail)
        {
            var item = _Dbcontext.BookingDetails
                .Include(s => s.Room)
                .Include(s => s.Booking)
                .Include(s => s.Guests)
                .Include(s => s.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .Where(s => s.BookingDetailId == idbookingdetail)
                .Select(s => new RoomPassengers()
                {
                    RoomName = s.Room.RoomNumber,
                    Status = s.Booking.Status,
                    NumberofRoom = s.Room.RoomNumber,

                    Passengers = s.Guests.Select(g => new Guests()
                    {
                        FullName = g.FullName,
                        Gender = g.Gender,
                        CodePersonal = g.CodePersonal,
                        PhoneNumber = g.PhoneNumber,
                        BirthDay = g.BirthDay,
                    }).ToList(),

                    Services = s.BookingServices.Select(bs => new Services()
                    {
                        ServiceName = bs.Service.ServiceName,
                        Price = bs.Service.Price,
                        Description = bs.Service.Description,
                        Discount = bs.Service.Discount
                    }).ToList()
                }).FirstOrDefault();


            return item;
        }

    }
}
