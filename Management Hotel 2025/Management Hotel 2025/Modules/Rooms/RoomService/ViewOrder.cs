using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public class ViewOrder : IOrder
    {
        private readonly ManagermentHotelContext _dbcontext;

        public ViewOrder(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // xác nhận checkout 
        public async Task<bool> ConfirmCheckOut(Order order, string OrdersMethod, int idStaff)
        {
            var booking = await _dbcontext.Bookings
                .FirstOrDefaultAsync(b => b.BookingCode == order.BookingCode);



            if (booking == null)
                return false;


            booking.Status = "CheckOut";
            booking.RealTimeCheckOut = DateTime.Now.Date;

            string Code = DateTime.Now.ToString("ddMMyyyyHHmmss") + new Random().Next(100, 999);

            var newOrder = new MyData.Models.Order
            {
                OrderDate = DateTime.Now,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress ?? "",
                CustomerPhone = order.CustomerPhone,
                Email = order.Email,
                Deposit = order.Deposit.ToString(),
                TotalAmount = order.TotalAmountOrder,
                OrderStatus = "Completed",
                PaymentMethod = OrdersMethod,
                IdStaff = idStaff,
                Booking = booking,
                OrderCode = "PTD" + Code
            };

            _dbcontext.Orders.Add(newOrder);

            return await _dbcontext.SaveChangesAsync() > 0;
        }



        // xem hóa đơn đạt phòng
        async Task<Order> IOrder.ViewOrder(string bookingcode)
        {
            var booking = await _dbcontext.Bookings.Include(s => s.Orders)
                .Include(s => s.BookingDetails)
                .ThenInclude(s => s.Room)
                .ThenInclude(s => s.RoomType)
                .Include(s => s.BookingDetails)
                .ThenInclude(s => s.BookingServices)
                .FirstOrDefaultAsync(o => o.BookingCode == bookingcode);

            if (booking == null) return new Order();

            var order = new Order
            {
                TimeDeposit = booking.BookingDate,
                CustomerName = booking.CustomerName,
                CustomerPhone = booking.CustomerPhone,
                PersonalId = booking.PersonalCode ?? "",
                Email = booking.Email,
                CustomerAddress = booking.Address,
                OrderDate = DateTime.Now,
                BookingCode = booking.BookingCode ?? "",
                RealCheckInDate = booking.RealTimeCheckIn ?? DateTime.MinValue,
                RealCheckOutDate = booking.RealTimeCheckOut ?? DateTime.MinValue,
                Deposit = booking.DepositAmount,
                OrderStatus = booking.Orders?.OrderStatus ?? "Pending",
                roomsOrders = booking.BookingDetails.Select(ro => new RoomOrder
                {
                    RoomType = ro.Room.RoomType.Name,
                    RoomNumber = ro.Room.RoomNumber,
                    PricePerNight = ro.Room.RoomType.Price,
                    NumberOfNights = ro.CheckOutDate.HasValue && ro.CheckInDate.HasValue
    ? (ro.CheckOutDate.Value - ro.CheckInDate.Value).Days
    : 0,
                    UsedToServices = ro.BookingServices.Select(bs => new ServiceToUsed()
                    {
                        ServiceName = bs.Service.ServiceName,
                        UnitPrice = bs.UnitPrice,
                        Quantity = bs.Quantity
                    }).ToList()
                }).ToList()
            };

            return order;
        }
    }

}
