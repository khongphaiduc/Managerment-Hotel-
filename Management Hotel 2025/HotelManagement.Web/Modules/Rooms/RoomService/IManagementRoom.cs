using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public interface IManagementRoom
    {

        public void CreateRoom();
        public void UpdateRoom();
        public void DeleteRoom();
      
        public void ViewDetailRoom(int idRoom);

        public Task<List<ViewRoomModel>> FilterRoom(string Option, int? Floor, DateTime startdate, DateTime enddate);

        public Task<ViewRoomModel> FilterByIdRoom(string IdRoom);

        public List<BookingInfo> GetListDateBookingOfRoom(int IdRoom);


        public List<MapRoom> getListMapRoomToDay();


        public RoomPassengers ViewDetailRoomPassengers(int idbookingdetail);
    }
}
