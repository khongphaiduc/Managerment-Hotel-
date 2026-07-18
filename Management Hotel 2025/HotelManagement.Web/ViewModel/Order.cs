using Management_Hotel_2025.Modules.Rooms.RoomService;
using Mydata.Models;

namespace Management_Hotel_2025.ViewModel
{
    public class Order
    {      
       
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string PersonalId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? CustomerAddress { get; set; }

        public DateTime TimeDeposit { get; set; }

        public DateTime OrderDate { get; set; }
        public string BookingCode { get; set; } = null!;

        public ICollection<RoomOrder> roomsOrders { get; set; } = new List<RoomOrder>();

        public DateTime RealCheckInDate { get; set; }
        public DateTime RealCheckOutDate { get; set; }

        public decimal Deposit { get; set; }

       
        public decimal TotalAmountOrder => roomsOrders.Sum(s => s.TotalAmount) - Deposit;

        public decimal TotalServicePrice => roomsOrders.Sum(s => s.TotalService);

        public decimal TotalRoomPrice => roomsOrders.Sum(s => s.TotalAmountRoom);

        public string OrderStatus { get; set; } = "Pending";

    }


    public class RoomOrder
    {
        public string RoomType { get; set; } = null!;
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public int NumberOfNights { get; set; }

        public ICollection<ServiceToUsed> UsedToServices { get; set; } = new List<ServiceToUsed>();

        public decimal TotalService => UsedToServices.Sum(s => s.UnitPrice * s.Quantity);

        public decimal TotalAmountRoom => PricePerNight * NumberOfNights;

        public decimal TotalAmount => TotalAmountRoom + TotalService;
    }


    public  class ServiceToUsed
    {
        public string ServiceName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }


}
