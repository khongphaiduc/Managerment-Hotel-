using Management_Hotel_2025.ViewModel;

namespace Management_Hotel_2025.Modules.Rooms.RoleAdmin.AdminServices
{
    public interface IAdminManagement
    {
        public AdminListsViewRoom ViewListRoom();

        public List<ViewRoomModel> ViewTypeRoom();

        public List<ViewRoomModel> SearchRoom(int? floor, string? status, string? key);


        public Task<bool> HideRoom (int idRoom);

        public AdJustRoom LoadTypeRoomAndAmentity();


        public List<int> NumberOfFloor();

        public List<string> StatusRoom();


    }
}
