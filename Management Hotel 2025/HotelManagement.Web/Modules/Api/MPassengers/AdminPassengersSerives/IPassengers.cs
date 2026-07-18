using API_BookingHotel.Modules.MPassengers.AdminPassengersModels;

namespace API_BookingHotel.Modules.MPassengers.AdminPassengersSerives
{
    public interface IPassengers
    {
        public Task<PassengerInfo> GetPassengerInfo(string PassengerCode, string hostapi);

    }
}
