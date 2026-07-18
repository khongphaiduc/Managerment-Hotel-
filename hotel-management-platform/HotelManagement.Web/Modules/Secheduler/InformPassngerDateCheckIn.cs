using AspNetCoreGeneratedDocument;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using Quartz;

namespace Management_Hotel_2025.Modules.Secheduler
{
    public class InformPassngerDateCheckIn : IJob
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _Inotification;
        private readonly IConfiguration _Iconfi;
        private string HOTEL_NAME;
        private string HOTEL_ADDRESS;
        private string CHECK_IN_TIME = "14:00 PM";
        private string BOOKING_DETAILS_URL = "https://yourhotelwebsite.com/booking-details";

        private string BOOKING_DETAILS_URL_WITH_PLACEHOLDER = "https://yourhotelwebsite.com/booking-details/{bookingId}";

        public InformPassngerDateCheckIn(ManagermentHotelContext dbcontext, INotifications notifications, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _Inotification = notifications;
            _Iconfi = configuration;


            HOTEL_NAME = _Iconfi["HotelInfo:HotelName"];
            HOTEL_ADDRESS = _Iconfi["HotelInfo:HotelAddress"];

        }

        public async Task Execute(IJobExecutionContext context)
        {


            var CheckIns = await _dbcontext.Bookings.Include(s => s.BookingDetails)
                .Where(s => s.Status == "CheckIn")
                .ToListAsync();

            foreach (var items in CheckIns)
            {
                var booking = items.BookingDetails.FirstOrDefault();

                if (booking != null)
                {
                    if (booking.CheckInDate.HasValue)
                    {

                        string body = $@"
    <!DOCTYPE html>
    <html lang=""vi"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Thông báo Check-in Sắp tới</title>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
            .header {{ background-color: #007bff; color: #ffffff; padding: 20px; text-align: center; }}
            .header h1 {{ margin: 0; font-size: 24px; }}
            .content {{ padding: 30px; line-height: 1.6; color: #333333; }}
            .info-box {{ background-color: #f9f9f9; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0; }}
            .cta-button {{ display: inline-block; background-color: #28a745; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 5px; font-weight: bold; margin-top: 20px; }}
            .footer {{ background-color: #eeeeee; color: #777777; padding: 20px; text-align: center; font-size: 12px; border-top: 1px solid #e0e0e0; }}
            .highlight {{ color: #007bff; font-weight: bold; }}
        </style>
    </head>
    <body>
        <div class=""container"">
            <div class=""header"">
                <h1>{HOTEL_NAME}</h1>
            </div>

            <div class=""content"">
                <p style=""font-size: 16px;"">**Kính gửi {items.CustomerName}**,</p>
                
                <p>Chúng tôi rất vui mừng thông báo rằng lịch check-in của bạn sắp đến. Đây là thông tin chi tiết về kỳ nghỉ của bạn tại **{HOTEL_NAME}**.</p>

                <div class=""info-box"">
                    <p style=""margin-top: 0;""><span class=""highlight"">Ngày Check-in:</span> **{items.BookingDetails.FirstOrDefault().CheckInDate}**</p>
                    <p><span class=""highlight"">Thời gian Check-in Tiêu chuẩn:</span> {CHECK_IN_TIME}</p>
                    <p style=""margin-bottom: 0;""><span class=""highlight"">Địa chỉ Khách sạn:</span> {HOTEL_ADDRESS}</p>
                </div>
                
                <p>Xin lưu ý mang theo giấy tờ tùy thân (CCCD/Hộ chiếu) khi làm thủ tục.</p>

                <p style=""text-align: center;"">
                    <a href=""{BOOKING_DETAILS_URL}"" class=""cta-button"" style=""color: #ffffff !important; text-decoration: none;"">
                        Xem Chi Tiết Đặt Phòng
                    </a>
                </p>
                
                <p>Nếu có bất kỳ thay đổi nào trong kế hoạch, vui lòng liên hệ với chúng tôi sớm nhất có thể.</p>

                <p>Trân trọng,</p>
                <p>Đội ngũ Dịch vụ Khách hàng **{HOTEL_NAME}**</p>
            </div>

            <div class=""footer"">
                <p>Đây là email tự động, vui lòng không trả lời.</p>
                <p>{HOTEL_NAME} | {HOTEL_ADDRESS}</p>
            </div>
        </div>
    </body>
    </html>";


                        if (DateTime.Now.Date == booking.CheckInDate.Value.AddDays(-1).Date)
                        {
                            await _Inotification.SendBookingSuccessNotification(items.Email, "Thông báo tới quý khách", body);
                        }
                    }
                }

            }

        }
    }
}
