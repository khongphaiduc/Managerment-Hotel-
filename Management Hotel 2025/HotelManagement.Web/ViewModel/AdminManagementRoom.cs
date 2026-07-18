using Microsoft.Identity.Client;

namespace Management_Hotel_2025.ViewModel
{
    public class AdminManagementRoom
    {
        public List<int> ListFloor { get; set; } = new List<int>();

        public List<string> ListStatusRoom { get; set; } = new List<string>();

        public List<ViewRoomModel> ListViewRooms { get; set; } = new List<ViewRoomModel>();

    }
}

