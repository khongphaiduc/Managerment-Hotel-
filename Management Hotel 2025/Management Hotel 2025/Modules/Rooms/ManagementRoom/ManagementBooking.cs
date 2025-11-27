using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using System.ComponentModel.DataAnnotations;

namespace Management_Hotel_2025.Modules.Rooms.ManagementRoom
{
    public class ManagementBooking : IManagementBooking
    {
        private readonly ManagermentHotelContext _Dbcontext;

        public ManagementBooking(ManagermentHotelContext Dbcontext)
        {
            _Dbcontext = Dbcontext;
        }

        public List<BookingItem> GetListBooking(DateTime? DateStart, DateTime? EndDate)
        {

            if (!DateStart.HasValue || !EndDate.HasValue)
            {
                DateStart = DateTime.Now.AddMonths(-1);
                EndDate = DateTime.Now.AddMonths(1);
            }
            else
            {
                var List = _Dbcontext.Bookings.Include(s => s.BookingDetails).Where(s => s.BookingDate >= DateStart && s.BookingDate <= EndDate)
                                         .Select(s => new BookingItem()
                                         {

                                             Code = s.BookingCode ?? "00",
                                             CustomerName = s.CustomerName,
                                             Phone = s.CustomerPhone,
                                             Email = s.Email,
                                             CheckIn = (DateTime)s.BookingDetails.Select(x => x.CheckInDate).FirstOrDefault(),
                                             CheckOut = (DateTime)s.BookingDetails.Select(x => x.CheckOutDate).FirstOrDefault(),
                                             RoomCount = s.BookingDetails.Count,
                                             Status = s.Status,
                                             BookingDate = s.BookingDate,


                                         }).OrderByDescending(s => s.BookingDate).ToList();

                return List;
            }

            return new List<BookingItem>();

        }

        // search by code booking 
        public List<BookingItem> SearchByBookingCode(string search)
        {
            search = search.Trim().ToLower();
            var List = _Dbcontext.Bookings.Include(s => s.BookingDetails).Where(s => s.BookingCode.Equals(search))
                                    .Select(s => new BookingItem()
                                    {

                                        Code = s.BookingCode,
                                        CustomerName = s.CustomerName,
                                        Phone = s.CustomerPhone,
                                        Email = s.Email,
                                        CheckIn = (DateTime)s.BookingDetails.Select(x => x.CheckInDate).FirstOrDefault(),
                                        CheckOut = (DateTime)s.BookingDetails.Select(x => x.CheckOutDate).FirstOrDefault(),
                                        RoomCount = s.BookingDetails.Count,
                                        Status = s.Status,
                                        BookingDate = s.BookingDate,


                                    }).ToList();
            return List;
        }

        //  search by name 
        public List<BookingItem> SearchByBookingName(string search)
        {
            search = search.Trim().ToLower();
            var List = _Dbcontext.Bookings.Include(s => s.BookingDetails).Where(s => s.CustomerName.Contains(search))
                                    .Select(s => new BookingItem()
                                    {

                                        Code = s.BookingCode,
                                        CustomerName = s.CustomerName,
                                        Phone = s.CustomerPhone,
                                        Email = s.Email,
                                        CheckIn = (DateTime)s.BookingDetails.Select(x => x.CheckInDate).FirstOrDefault(),
                                        CheckOut = (DateTime)s.BookingDetails.Select(x => x.CheckOutDate).FirstOrDefault(),
                                        RoomCount = s.BookingDetails.Count,
                                        Status = s.Status,
                                        BookingDate = s.BookingDate,


                                    }).ToList();
            return List;
        }


        // search by phone
        public List<BookingItem> SearchByBookingPhone(string search)
        {
            search = search.Trim().ToLower();
            var List = _Dbcontext.Bookings.Include(s => s.BookingDetails).Where(s => s.CustomerPhone.Equals(search))
                                    .Select(s => new BookingItem()
                                    {

                                        Code = s.BookingCode,
                                        CustomerName = s.CustomerName,
                                        Phone = s.CustomerPhone,
                                        Email = s.Email,
                                        CheckIn = (DateTime)s.BookingDetails.Select(x => x.CheckInDate).FirstOrDefault(),
                                        CheckOut = (DateTime)s.BookingDetails.Select(x => x.CheckOutDate).FirstOrDefault(),
                                        RoomCount = s.BookingDetails.Count,
                                        Status = s.Status,
                                        BookingDate = s.BookingDate,


                                    }).ToList();
            return List;
        }


        // search by email
        public List<BookingItem> SearchByBookingEmail(string search)
        {
            search = search.Trim().ToLower();
            var List = _Dbcontext.Bookings.Include(s => s.BookingDetails).Where(s => s.Email.Equals(search))
                                    .Select(s => new BookingItem()
                                    {

                                        Code = s.BookingCode,
                                        CustomerName = s.CustomerName,
                                        Phone = s.CustomerPhone,
                                        Email = s.Email,
                                        CheckIn = (DateTime)s.BookingDetails.Select(x => x.CheckInDate).FirstOrDefault(),
                                        CheckOut = (DateTime)s.BookingDetails.Select(x => x.CheckOutDate).FirstOrDefault(),
                                        RoomCount = s.BookingDetails.Count,
                                        Status = s.Status,
                                        BookingDate = s.BookingDate,


                                    }).ToList();
            return List;
        }



        public List<BookingItem> GetListBooking(string search)
        {


            var list1 = SearchByBookingCode(search);
            var list2 = SearchByBookingName(search);
            var list3 = SearchByBookingPhone(search);
            var list4 = SearchByBookingEmail(search);

            if (list1.Count > 0)
                return list1 = list1.OrderByDescending(s => s.BookingDate).ToList();
            else if (list2.Count > 0)
                return list2 = list2.OrderByDescending(s => s.BookingDate).ToList();
            else if (list3.Count > 0)
                return list3 = list3.OrderByDescending(s => s.BookingDate).ToList();
            else if (list4.Count > 0)
                return list4 = list4.OrderByDescending(s => s.BookingDate).ToList();

            return new List<BookingItem>();

        }


        // xem chi tiết booking 
        public ViewBookingDetail ViewDetailBooking(string BookingCode)
        {

            var ViewBookingDetail = _Dbcontext.Bookings.Include(s => s.BookingDetails)
                 .ThenInclude(s => s.Room)
                 .ThenInclude(s => s.RoomType)
                 .Where(s => s.BookingCode.Equals(BookingCode))
                 .Select(s => new ViewBookingDetail()
                 {
                     BookingCode = s.BookingCode,
                     CreatedDate = s.BookingDate,
                     BookingStatus = s.Status,
                     CustomerFullName = s.CustomerName,
                     CustomerPhone = s.CustomerPhone,
                     CustomerEmail = s.Email,
                     NumberOfRoom = s.BookingDetails.Count,
                     BookingSource = s.BookingSource,
                     ListDetailRoom = s.BookingDetails.Select(bd => new ViewDetailRoom()
                     {
                         RoomNumber = bd.Room.RoomNumber,
                         NameType = bd.Room.RoomType.Name,
                         Price = bd.Room.RoomType.Price,
                         Status = bd.Room.Status,
                         MaxGuests = bd.Room.RoomType.MaxGuests.ToString(),
                         Description = bd.Room.Description,

                     }).ToList(),

                     CheckInDate = (DateTime)s.BookingDetails.Select(bd => bd.CheckInDate).FirstOrDefault(),
                     CheckOutDate = (DateTime)s.BookingDetails.Select(bd => bd.CheckOutDate).FirstOrDefault(),
                     TotalAmountRoom = s.TotalAmountBooking,
                     DepositAmount = s.DepositAmount,
                     Discount = 0,
                     PaymentMethod = "Credit Card",

                 }).FirstOrDefault();


            return ViewBookingDetail ?? new ViewBookingDetail()
            {
                BookingCode = "Booking không tồn tại.",
            };
        }
    }
}
