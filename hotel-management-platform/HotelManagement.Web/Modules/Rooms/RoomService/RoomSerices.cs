

using Management_Hotel_2025.Modules.Rooms.RoomViewModel;

using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public class RoomSerices : IRoomService
    {
        private readonly ManagermentHotelContext _dbcontext;
 
        public RoomSerices(ManagermentHotelContext managermentHotelContext)
        {
            _dbcontext = managermentHotelContext;
        }

        public bool AddServicesHotel(int IdService)
        {
            throw new NotImplementedException();
        }
        public List<ViewListRoomsOfUser> GetListRoomOfUser(int userId)
        {
            var listItem = _dbcontext.Bookings
       .Include(s => s.BookingDetails)
           .ThenInclude(d => d.Room)
               .ThenInclude(r => r.RoomType)
       .Where(s => s.UserId == userId && s.Status == "Success")
       .Select(s => new ViewListRoomsOfUser()
       {
           IdRoom = s.BookingDetails.Select(b => b.Room.RoomId).FirstOrDefault(),
           NumberRoom = Convert.ToInt32(s.BookingDetails.Select(s => s.Room.RoomNumber).FirstOrDefault()),
           NameRoom = s.BookingDetails.Select(b => b.Room.RoomType.Name).FirstOrDefault(),
           StatusRoom = s.BookingDetails.Select(b => b.StatusCheckRoom).FirstOrDefault(),
           Floor = s.BookingDetails.Select(b => b.Room.Floor).FirstOrDefault(),
           PriceRoom = s.BookingDetails.Select(b => b.Room.RoomType.Price).FirstOrDefault(),
           DescriptionRoom = s.BookingDetails.Select(b => b.Room.Description).FirstOrDefault(),
           ImageRoom = s.BookingDetails.Select(b => b.Room.PathImage).FirstOrDefault(),
           DateCheckIn = s.BookingDetails.Select(b => b.CheckInDate).FirstOrDefault(),
           DateCheckout = s.BookingDetails.Select(b => b.CheckOutDate).FirstOrDefault(),

       }).ToList();


            if (listItem.Count == 0)
            {
                return new List<ViewListRoomsOfUser>();
            }
            else
            {
                return listItem;
            }
        }

        public async Task<ViewDetailRoom> ViewDetailOfRoom(int IdRoom)
        {
            var room = await _dbcontext.Rooms
                .AsNoTracking()
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.RoomId == IdRoom);

            if (room == null)
            {
                return new ViewDetailRoom();
            }

            return new ViewDetailRoom
            {
                RoomId = room.RoomId,
                RoomTypeId = room.RoomTypeId,
                NameType = room.RoomType.Name,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor ?? 0,
                Status = room.Status,
                Description = room.Description,
                PathImage = room.PathImage,
                Price = room.RoomType.Price,
                MaxGuests = room.RoomType.MaxGuests.ToString(),
                ListPathImage = room.Images.Select(i => i.LinkImage).ToList(),
                ListAmenites = room.RoomAmenities
                    .Where(ra => ra.Amenity.status == "Active")
                    .Select(ra => ra.Amenity)
                    .ToList()
            };
        }
    }
}
