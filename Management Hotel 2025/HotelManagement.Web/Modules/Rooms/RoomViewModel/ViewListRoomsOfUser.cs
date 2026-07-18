using System.Runtime.InteropServices;

namespace Management_Hotel_2025.Modules.Rooms.RoomViewModel
{
    public class ViewListRoomsOfUser
    {

        public int IdRoom { get; set; }

        public int NumberRoom { get; set; }

        public string? NameRoom { get; set; }

        public string? TypeRoom { get; set; }

        public string? StatusRoom { get; set; }


        public decimal? PriceRoom { get; set; }

        public string? DescriptionRoom { get; set; }

        public string? ImageRoom { get; set; }

        public DateTime? DateCheckIn { get; set; }

        public DateTime? DateCheckout { get; set; }

        public int? Floor { get; set; }

    }
}
