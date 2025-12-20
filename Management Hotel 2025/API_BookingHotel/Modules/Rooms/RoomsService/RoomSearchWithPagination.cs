
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


        // tìm kiếm advance của room
         public async Task<List<ViewRoom>> SearchRoomByAdvance(RoomFilterRequest roomRequest, string apihost)
        {

            var ItemSkip = (roomRequest.PageCurrent - 1) * roomRequest.NumerItemOfPage; // số lượng item sẽ bỏ qua

            DateTime newCheckIn = DateTime.Parse(roomRequest.StartDate!);
            DateTime newCheckOut = DateTime.Parse(roomRequest.EndDate!);

            var ListItem = await _dbcontext.Rooms
                          .Include(s => s.RoomType)
                          .Where(s =>  (s.Status == "Active")&& (!roomRequest.Floor.HasValue || s.Floor == roomRequest.Floor.Value) &&
                                (!roomRequest.PriceMin.HasValue || s.RoomType.Price >= roomRequest.PriceMin.Value) &&
                                (!roomRequest.PriceMax.HasValue || s.RoomType.Price <= roomRequest.PriceMax.Value) &&
                                (!roomRequest.Person.HasValue || s.RoomType.MaxGuests == roomRequest.Person.Value) &&
                                 !s.BookingDetails.Any(bd =>                                                 // cú  pháp Any (điều kiện) :
                                                                                                             // Dùng để kiểm  trả 1 phần tử hay danh sách có ít nhất item thỏa mãn hay không
                                                       bd.Booking.Status != "Cancelled" &&                   // nếu có ít nhất 1 item thỏa mãn thì  sẽ return TRUE và ngược lại FALSE   
                                                       newCheckIn < bd.CheckOutDate &&
                                                       newCheckOut > bd.CheckInDate)

                                )
                          .OrderBy(s => s.RoomType.Price)
                          .Skip(ItemSkip)                                     // bỏ qua số lượng Item cần skip
                          .Take(roomRequest.NumerItemOfPage)                            // lấy số  lượng item của 1 page
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


        // tìm kiếm advance của room của  thằng  management
        public async Task<List<ViewRoom>> SearchRoomByAdvanceForManagement(
        string option, int CurrentPage, int ItermNumberOfPage,
        int? Floor, int? PriceMin, int? PriceMax, int? Person,
        string? StartDate, string? EndDate)
        {


            var ItemSkip = (CurrentPage - 1) * ItermNumberOfPage; // số lượng item sẽ bỏ qua

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
                          .Skip(ItemSkip)                                     // bỏ qua số lượng Item cần skip
                          .Take(ItermNumberOfPage)                            // lấy số  lượng item của 1 page
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
