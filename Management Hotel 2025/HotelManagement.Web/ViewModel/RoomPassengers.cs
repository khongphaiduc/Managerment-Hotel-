using Mydata.Models;


namespace Management_Hotel_2025.ViewModel
{
    public class RoomPassengers
    {

        public string RoomName { get; set; }

        public string NumberofRoom { get; set; }
        public string Status { get; set; }

        public List<Guests> Passengers { get; set; }
        public List<Services> Services { get; set; }
    }
}
