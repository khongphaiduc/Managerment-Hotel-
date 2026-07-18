namespace Management_Hotel_2025.ViewModel
{
    public class BookingItem
    {

        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int RoomCount { get; set; }
        public string Status { get; set; } // e.g., "Confirmed","Pending","Cancelled"
        public DateTime BookingDate { get; set; }
   
    }
}
