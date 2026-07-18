
namespace Management_Hotel_2025.ViewModel
{
    public class AdminListsViewRoom
    {

        public ICollection<RoomTemporary> ListCheckInToday { get; set; } = new List<RoomTemporary>();

        public ICollection<RoomTemporary> ListCheckOutToday { get; set; } = new List<RoomTemporary>();


        public ICollection<RoomTemporary> ListCustomerUsing { get; set; } = new List<RoomTemporary>();

    }

    public class RoomTemporary
    {

        public string NameCustomer { get; set; } = "Không xác định ";

        public string TypeRoom { get; set; } = "Normal";

        public int NumberOfRoom { get; set; }

        public DateTime DayCheckIn { get; set; }

        public DateTime DayCheckOut { get; set; }

        public string TypeCustomer { get; set; } = "Sigle";


    }
}
