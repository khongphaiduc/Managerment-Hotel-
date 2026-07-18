using Management_Hotel_2025.Modules.Rooms.RoomViewModel;
using Management_Hotel_2025.ViewModel;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public interface IRoomService
    {
     
        public List<ViewListRoomsOfUser> GetListRoomOfUser(int userId);


        public Task<ViewDetailRoom> ViewDetailOfRoom(int IdRoom);


        public bool AddServicesHotel(int IdService);


    }
}
