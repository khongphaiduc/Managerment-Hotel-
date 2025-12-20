using API_BookingHotel.Modules.Rooms.DTOs;
using API_BookingHotel.ViewModels;

namespace API_BookingHotel.Modules.Rooms.RoomsService
{
    public interface IRoomService
    {

       
        public Task<List<ViewRoom>> SearchRoomByAdvance(RoomFilterRequest roomRequest, string apihost);
        public Task<List<ViewRoom>> SearchRoomByAdvanceForManagement(string option, int CurrentPage, int ItermNumberOfPage, int? Floor, int? PriceMin, int? PriceMax, int? Person, string StartDate, string EndDate);



    }
}





