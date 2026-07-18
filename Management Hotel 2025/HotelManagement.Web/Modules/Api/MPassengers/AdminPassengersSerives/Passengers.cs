using API_BookingHotel.Modules.MPassengers.AdminPassengersModels;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;

namespace API_BookingHotel.Modules.MPassengers.AdminPassengersSerives
{
    public class Passengers : IPassengers
    {
        private readonly ManagermentHotelContext _dbcontext;

        public Passengers(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<PassengerInfo> GetPassengerInfo(string PassengerCode, string hostapi)
        {

            var passenger = await _dbcontext.Bookings.Where(s => s.PersonalCode == PassengerCode)
            .Select(s => new PassengerInfo()
            {
                FullName = s.CustomerName,
                PassengerCode = s.PersonalCode,
                Email = s.Email,
                Phone = s.CustomerPhone,
                Address = s.Address,
                Bithday = s.BirthDay,
                Sex = s.Sex,
                Nationality = s.Nationality,
                UrlImage = $"{hostapi}/PassengersAvatars/non-avatar.jpg"

            }).FirstOrDefaultAsync();


           return passenger ?? new PassengerInfo() { PassengerCode = "0000" };
        }
    }
}
