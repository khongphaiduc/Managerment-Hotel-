using Management_Hotel_2025.ViewModel;

namespace Management_Hotel_2025.Modules.Rooms.ManagementRoom
{
    public interface IManagementBooking
    {

        public List<BookingItem> GetListBooking(DateTime? DateStart, DateTime? EndDate);
        public List<BookingItem> GetListBooking(string search);
        public ViewBookingDetail ViewDetailBooking(string BookingCode);
    }

   
}
