using API_BookingHotel.ViewModels;

namespace API_BookingHotel.Modules.Rooms.RoomsService
{
    public interface IEditableRoom
    {
        public Task<bool> EditRoomStatus(AdJustRoom room);


        public Task<AdJustRoom> GetFullInfoRoom(int roomId, string apihost);

        public Task<bool> CreateNewRoom(AdJustRoom room);

    }
}
