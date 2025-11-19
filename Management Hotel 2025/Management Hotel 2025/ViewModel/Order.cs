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

        public DateTime OrderDate { get; set; }    // thời gian tạo hóa đơn
        public string BookingCode { get; set; } = null!;  // mã đặt phòng

        public ICollection<RoomOrder> roomsOrders { get; set; } = new List<RoomOrder>(); // danh sách phòng đặt

        public DateTime RealCheckInDate { get; set; }   // ngày nhận phòng thực tế
        public DateTime RealCheckOutDate { get; set; }  // ngày trả phòng thực tế

        public decimal Deposit { get; set; }    // tiền cọc trước đó

       
        public decimal TotalAmountOrder => roomsOrders.Sum(s => s.TotalAmount) - Deposit; // tổng  tiền của hóa đơn

        public decimal TotalServicePrice => roomsOrders.Sum(s => s.TotalService);  // tổng tiền dịch vụ của các phòng 

        public decimal TotalRoomPrice => roomsOrders.Sum(s => s.TotalAmountRoom);  // tổng số tiền phòng của các phòng

        public string OrderStatus { get; set; } = "Pending";

    }


    public class RoomOrder
    {
        public string RoomType { get; set; } = null!;
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }   // giá phòng
        public int NumberOfNights { get; set; }      // số đêm

        public ICollection<ServiceToUsed> UsedToServices { get; set; } = new List<ServiceToUsed>();

        // Tính tổng dịch vụ động
        public decimal TotalService => UsedToServices.Sum(s => s.UnitPrice * s.Quantity);

        // Tính tổng tiền phòng
        public decimal TotalAmountRoom => PricePerNight * NumberOfNights;

        // Tính tổng tiền cả phòng và dịch vụ
        public decimal TotalAmount => TotalAmountRoom + TotalService;
    }


    public  class ServiceToUsed
    {
        public string ServiceName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }


}
