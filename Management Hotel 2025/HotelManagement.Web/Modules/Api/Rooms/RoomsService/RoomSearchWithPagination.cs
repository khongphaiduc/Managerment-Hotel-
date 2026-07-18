
using API_BookingHotel.Modules.Rooms.DTOs;
using API_BookingHotel.ViewModels;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;


namespace API_BookingHotel.Modules.Rooms.RoomsService
{
    public class RoomSearchWithPagination : IRoomService
    {
        private readonly ManagermentHotelContext _dbcontext;

        public RoomSearchWithPagination(ManagermentHotelContext Dbcontext)
        {
            _dbcontext = Dbcontext;
        }


         public async Task<List<ViewRoom>> SearchRoomByAdvance(RoomFilterRequest roomRequest, string apihost)
        {

            var ItemSkip = (roomRequest.PageCurrent - 1) * roomRequest.NumerItemOfPage;

            DateTime newCheckIn = DateTime.Parse(roomRequest.StartDate!);
            DateTime newCheckOut = DateTime.Parse(roomRequest.EndDate!);

            var ListItem = await _dbcontext.Rooms
                          .Include(s => s.RoomType)
                          .Where(s =>  (s.Status == "Active")&& (!roomRequest.Floor.HasValue || s.Floor == roomRequest.Floor.Value) &&
                                (!roomRequest.PriceMin.HasValue || s.RoomType.Price >= roomRequest.PriceMin.Value) &&
                                (!roomRequest.PriceMax.HasValue || s.RoomType.Price <= roomRequest.PriceMax.Value) &&
                                (!roomRequest.Person.HasValue || s.RoomType.MaxGuests == roomRequest.Person.Value) &&
                                 !s.BookingDetails.Any(bd =>
                                                       bd.Booking.Status != "Cancelled" &&
                                                       newCheckIn < bd.CheckOutDate &&
                                                       newCheckOut > bd.CheckInDate)

                                )
                          .OrderBy(s => s.RoomType.Price)
                          .Skip(ItemSkip)
                          .Take(roomRequest.NumerItemOfPage)
                          .Select(room => new ViewRoom()
                          {
                              IdRoom = room.RoomId,
                              Name = room.RoomType.Name,
                              Floor = (int)room.Floor!,
                              Description = room.Description,
                              Image = room.PathImage.StartsWith("http")? room.PathImage :$"{apihost}/AvatarImages/{room.PathImage}",
                              Price = room.RoomType.Price,
                              NumberOfRooms = room.RoomNumber
                          })
                             .ToListAsync();

            return ListItem;
        }

        public async Task<int> CountRoomByAdvance(RoomFilterRequest roomRequest)
        {
            DateTime newCheckIn = DateTime.Parse(roomRequest.StartDate!);
            DateTime newCheckOut = DateTime.Parse(roomRequest.EndDate!);

            return await _dbcontext.Rooms
                .Where(s => s.Status == "Active"
                    && (!roomRequest.Floor.HasValue || s.Floor == roomRequest.Floor.Value)
                    && (!roomRequest.PriceMin.HasValue || s.RoomType.Price >= roomRequest.PriceMin.Value)
                    && (!roomRequest.PriceMax.HasValue || s.RoomType.Price <= roomRequest.PriceMax.Value)
                    && (!roomRequest.Person.HasValue || s.RoomType.MaxGuests == roomRequest.Person.Value)
                    && !s.BookingDetails.Any(bd =>
                        bd.Booking.Status != "Cancelled"
                        && newCheckIn < bd.CheckOutDate
                        && newCheckOut > bd.CheckInDate))
                .CountAsync();
        }


        public async Task<List<ViewRoom>> SearchRoomByAdvanceForManagement(
        string option, int CurrentPage, int ItermNumberOfPage,
        int? Floor, int? PriceMin, int? PriceMax, int? Person,
        string? StartDate, string? EndDate)
        {


            var ItemSkip = (CurrentPage - 1) * ItermNumberOfPage;

            DateTime newCheckIn = DateTime.Parse(StartDate);
            DateTime newCheckOut = DateTime.Parse(EndDate);

            var ListItem = await _dbcontext.Rooms
                          .Include(s => s.RoomType)
                          .Where(s => (!Floor.HasValue || s.Floor == Floor.Value) &&
                                (!PriceMin.HasValue || s.RoomType.Price >= PriceMin.Value) &&
                                (!PriceMax.HasValue || s.RoomType.Price <= PriceMax.Value) &&
                                (!Person.HasValue || s.RoomType.MaxGuests == Person.Value)                                
                                )
                          .OrderBy(s => s.RoomType.Price)
                          .Skip(ItemSkip)
                          .Take(ItermNumberOfPage)
                          .Select(room => new ViewRoom()
                          {
                              IdRoom = room.RoomId,
                              Name = room.RoomType.Name,
                              Floor = (int)room.Floor,
                              Description = room.Description,
                              Image = room.PathImage,
                              Price = room.RoomType.Price,
                              NumberOfRooms = room.RoomNumber
                          })
                             .ToListAsync();

            return ListItem;
        }


    }
}
