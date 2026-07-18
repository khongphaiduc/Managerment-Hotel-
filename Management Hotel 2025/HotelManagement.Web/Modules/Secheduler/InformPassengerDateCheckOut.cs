using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using Quartz;

namespace Management_Hotel_2025.Modules.Secheduler
{
    public class InformPassengerDateCheckOut : IJob
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _Inotification;
        private readonly IConfiguration _Iconfi;
        private string HOTEL_NAME;
        private string HOTEL_ADDRESS;
        private string CHECK_OUT_TIME;
        private string HOTEL_PHONE;



        public InformPassengerDateCheckOut(ManagermentHotelContext dbcontext, INotifications notifications, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _Inotification = notifications;
            _Iconfi = configuration;


            HOTEL_NAME = _Iconfi["HotelInfo:HotelName"];
            HOTEL_ADDRESS = _Iconfi["HotelInfo:HotelAddress"];
            CHECK_OUT_TIME = _Iconfi["HotelInfo:CheckOutTime"];
            HOTEL_PHONE = _Iconfi["HotelInfo:Phone"];

        }
        public async Task Execute(IJobExecutionContext context)
        {

            var CheckOuts = await _dbcontext.Bookings.Include(s => s.BookingDetails)
                .Where(s => s.Status == "CheckOut")
                .ToListAsync();



            if (CheckOuts != null)
            {
                foreach (var item in CheckOuts)
                {



                    if (DateTime.Now.Date == item.BookingDetails.FirstOrDefault().CheckOutDate.Value.Date)
                    {
                        string body = $@"
    <!DOCTYPE html>
    <html lang=""vi"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Thông báo Trả phòng Sắp tới</title>
        <style>
            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
            .header h1 {{ margin: 0; font-size: 24px; }}
            .content {{ padding: 30px; line-height: 1.6; color: #333333; }}
            .info-box {{ background-color: #f9f9f9; padding: 15px; border-left: 4px solid #34495e; margin: 20px 0; }}
            .instruction-list {{ padding-left: 20px; }}
            .instruction-list li {{ margin-bottom: 10px; }}
            .footer {{ background-color: #eeeeee; color: #777777; padding: 20px; text-align: center; font-size: 12px; border-top: 1px solid #e0e0e0; }}
            .highlight {{ color: #34495e; font-weight: bold; }}
            .thank-you {{ font-size: 18px; font-weight: bold; color: #007bff; margin-top: 25px; }}
        </style>
    </head>
    <body>
        <div class=""container"">
            <div class=""header"">
                <h1>{HOTEL_NAME}</h1>
            </div>

            <div class=""content"">
                <p style=""font-size: 16px;"">**Kính gửi {item.CustomerName}**,</p>
                
                <p>Chúng tôi hy vọng bạn đã có một kỳ nghỉ tuyệt vời tại khách sạn của chúng tôi. Chúng tôi xin gửi thông báo về ngày trả phòng sắp tới.</p>

                <div class=""info-box"">
                    <p style=""margin-top: 0;""><span class=""highlight"">Ngày Trả phòng:</span> **{item.BookingDetails.FirstOrDefault().CheckOutDate.Value.Date}**</p>
                    <p style=""margin-bottom: 0;""><span class=""highlight"">Thời gian Trả phòng Tiêu chuẩn:</span> **{CHECK_OUT_TIME}**</p>
                </div>
                
                <p style=""font-weight: bold; margin-top: 25px;"">Thủ tục Trả phòng:</p>
                <ul class=""instruction-list"">
                    <li>Vui lòng kiểm tra lại đồ đạc cá nhân.</li>
                    <li>Hoàn trả chìa khóa phòng hoặc thẻ từ tại quầy Lễ tân.</li>
                    <li>Thanh toán các chi phí phát sinh (minibar, dịch vụ giặt là, bữa ăn, v.v.) trước khi rời đi.</li>
                </ul>
                
                <p>Nếu bạn cần hỗ trợ về vận chuyển, giữ hành lý hoặc muốn yêu cầu trả phòng muộn (phụ thuộc vào tình trạng phòng), vui lòng liên hệ trực tiếp với Lễ tân.</p>

                <p class=""thank-you"">Cảm ơn bạn đã lựa chọn {HOTEL_NAME}!</p>

                <p>Trân trọng,</p>
                <p>Đội ngũ Dịch vụ Khách hàng {HOTEL_NAME}</p>
            </div>

            <div class=""footer"">
                <p>Liên hệ: {HOTEL_PHONE} | Địa chỉ: {HOTEL_ADDRESS}</p>
                <p>Chúng tôi mong được phục vụ bạn trong những lần tiếp theo.</p>
            </div>
        </div>
    </body>
    </html>";

                        await _Inotification.SendBookingSuccessNotification(item.Email, "Thông báo Trả phòng Sắp tới", body);
                    }

                }
            }
        }
    }
}
