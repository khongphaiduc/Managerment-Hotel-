using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using System.Net;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public class ReceptionService : IReceptionService
    {
        private readonly ManagermentHotelContext _dbcontext;

        public ReceptionService(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public Booking CheckIn(string BookingCode)
        {
            var booking = _dbcontext.Bookings.Include(s => s.BookingDetails)
                .Where(s => s.BookingCode.EndsWith(BookingCode))
                .Select(s => new Booking()
                {
                    BookingCode = s.BookingCode,
                    PersonalCode = s.PersonalCode,
                    Nationality = s.Nationality,
                    BirthDay = s.BirthDay,
                    CustomerPhone = s.CustomerPhone,
                    CustomerName = s.CustomerName,
                    Email = s.Email,
                    Address = s.Address,
                    TypePassenger = s.TypePassenger,
                    BookingDetails = s.BookingDetails,
                    Status = s.Status
                }).FirstOrDefault();

            return booking ?? new Booking() { BookingCode = "0" };
        }


        public bool CheckIn(Booking s)
        {
          
            if (s == null || string.IsNullOrEmpty(s.BookingCode))
            {
                return false;
            }

          
            var item = _dbcontext.Bookings.FirstOrDefault(t => t.BookingCode == s.BookingCode);

      
            if (item == null)
            {
                return false;
            }


            item.BookingCode = s.BookingCode;
            item.PersonalCode = s.PersonalCode;
            item.Nationality = s.Nationality;
            item.BirthDay = s.BirthDay;
            item.CustomerPhone = s.CustomerPhone;
            item.CustomerName = s.CustomerName;
            item.Email = s.Email;
            item.Address = s.Address;
            item.TypePassenger = s.TypePassenger;
            item.Status = "CheckIn";
            item.RealTimeCheckIn = DateTime.Now;    


            return _dbcontext.SaveChanges() > 0;
        }

        public Booking Checkout(Booking booking)
        {
            throw new NotImplementedException();
        }

        public bool RegisterGuestInfo(Guests guests)
        {
            _dbcontext.Add(guests);
            return _dbcontext.SaveChanges() > 0;

        }
    }
}
