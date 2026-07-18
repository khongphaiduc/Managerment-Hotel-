using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using Quartz;
using static QRCoder.PayloadGenerator;

namespace Management_Hotel_2025.Modules.Secheduler
{
    public class RefreshStatusRoom : IJob
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _notification;
        private readonly IConfiguration _config;


        private readonly string hotelName;
        private readonly string phoneNumber;
        private readonly string contactEmail;
        private string TitielEmailCancel = "Thông báo hủy đặt phòng - TrungDuc Luxury Hotel";

        public RefreshStatusRoom(ManagermentHotelContext dbcontext, INotifications notifications, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _notification = notifications;
            _config = configuration;


            hotelName = _config["InfoHotel:HotelName"];
            phoneNumber = _config["InfoHotel:Phone"];
            contactEmail = _config["InfoHotel:Email"];
        }




        public async Task Execute(IJobExecutionContext context)
        {
            var items = await _dbcontext.BookingDetails
                .Include(s => s.Booking)
                .Where(s => s.Booking.Status == "Success")
                .ToListAsync();

            foreach (var bookingDetail in items)
            {
                if (DateTime.Now.Date > bookingDetail.CheckInDate)
                {
                    bookingDetail.Booking.Status = "Cancelled";
                    string contentCancel = $@"
<p>Kính gửi <strong>{bookingDetail.Booking.CustomerName}</strong>,</p>
<p>Theo thông tin đặt phòng của Quý khách tại <strong>{hotelName}</strong>, 
thời gian nhận phòng dự kiến là <strong>{bookingDetail.CheckInDate:HH:mm, dd/MM/yyyy}</strong>. 
Tuy nhiên, đến thời điểm hiện tại, Quý khách vẫn chưa đến nhận phòng.</p>

<p>Rất tiếc, do Quý khách không đến trong thời gian quy định và không có thông báo tới chúng tôi, 
đặt phòng của Quý khách đã được <strong>hủy tự động</strong> theo chính sách của khách sạn.</p>

<p>Nếu Quý khách vẫn có nhu cầu lưu trú, vui lòng thực hiện 
<strong>đặt phòng mới</strong> qua website hoặc liên hệ trực tiếp với chúng tôi 
qua số <strong>{phoneNumber}</strong> để được hỗ trợ.</p>

<p>Chúng tôi rất mong có cơ hội được phục vụ Quý khách trong thời gian tới.</p>

<p>Trân trọng,<br/>
<strong>{hotelName}</strong><br/>
📞 {phoneNumber} | ✉️ {contactEmail}</p>";

                    await _notification.SendBookingSuccessNotification(bookingDetail.Booking.Email, TitielEmailCancel, contentCancel);
                }
            }

            await _dbcontext.SaveChangesAsync();
        }



    }
}
