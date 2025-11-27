using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// bảng tạm để lưu thông tin đặt phòng khi thanh toán qua payos  
// khi user tạo order thì order Paymentcode của user lưu vào Description của payos , cùng lúc đó tạo 1 bản ghi trong bảng tạm với Paymentcode đó,
// khi payos gửi webhook xác nhận thanh toán thành công thì lấy Paymentcode từ webhook so sánh với bảng tạm để lấy thông tin đặt phòng và lưu vào bảng chính thức
namespace MyData.Models
{
    [Table("BookingTemporary")]
    public class BookingTemporaryPayOS
    {
        [Key]
        public int Id { get; set; }

        public string? PaymentCode { get; set; }

        public int? IdRoom { get; set; }

        public int? NumberOFRoom { get; set; }

        public decimal? DepositAmount { get; set; }

        public string? NameCustomer { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Nationality { get; set; }

        public string? Email { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
