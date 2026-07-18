using MyData.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mydata.Models
{
    public class BookingService
    {
        public int BookingServiceId { get; set; }

        public int BookingDetailId { get; set; }   // năm  phòng nào 

        public int ServiceId { get; set; }   // những dịch vụ sử dụng 

        public decimal UnitPrice { get; set; }  // giá dịch vụ tại thời điểm dartwd sau khi mà trừ discount

        public int Quantity { get; set; }  // số lượng dịch vụ sử dụng
        

        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [ForeignKey("BookingDetailId")]

        public virtual BookingDetail BookingDetail { get; set; } = null!;
        [ForeignKey("ServiceId")]

        public virtual Services Service { get; set; } = null!;



        public int? OrderId { get; set; }

        [ForeignKey("OrderId")]

        public virtual Order? Order { get; set; }

    }
}
