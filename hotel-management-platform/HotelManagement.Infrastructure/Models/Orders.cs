using Mydata.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyData.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [ForeignKey("Booking")]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }    // thời gian tạo đơn

        public string OrderCode { get; set; }    // mã đơn
        public int IdStaff { get; set; }          // nhân viên tạo đơn

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerPhone { get; set; }

        public string Email { get; set; }

        public string Deposit { get; set; }       // tiền đặt cọc

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; }   // trạng thái đơn

        public string PaymentMethod { get; set; }
        public ICollection<BookingService>? BookingServices { get; set; }

        public virtual Booking Booking { get; set; }

    }
}
