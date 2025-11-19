using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MyData.Models;
using System;
using System.Collections.Generic;

namespace Mydata.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }   // có thể null nếu là khách walk-in

    public string? BookingCode { get; set; } // mã code booking để khách checkin   

    public string BookingSource { get; set; } = "Website";   // Website, App, Phone, WalkIn, TravelAgency    

    public string Status { get; set; } = "Pending";   // Pending, Confirmed, Cancelled, CheckedIn, CheckedOut

    public DateTime BookingDate { get; set; } = DateTime.Now;

    public decimal DepositAmount { get; set; } = 0;  // số tiền mà khách đã thành toán khi đặt cọc phòng 

    public decimal TotalAmountBooking { get; set; }  // tổng số tiền của booking tiền phòng chưa bao gom dịch vụ


    public string CustomerName { get; set; } = null!;  // tên khách chính trong booking

    public string CustomerPhone { get; set; } = null!; // số điện thoại khách chính trong booking

    public string? Sex { get; set; }

    public DateTime? BirthDay { get; set; }

    public string? PersonalCode { get; set; }

    public int? AmountPersonal { get; set; }

    public string Nationality { get; set; } = "VietNam";

    public string Email { get; set; } = null!;


    public string? Address { get; set; }

    public string TypePassenger { get; set; } = "Personal";



     //thời điểm khách check-in thực tế 
    public DateTime? RealTimeCheckIn { get; set; }


    // thơì điểm khách check-out thực tế
    public DateTime? RealTimeCheckOut { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    // 
    public virtual Order? Orders { get; set; }
}