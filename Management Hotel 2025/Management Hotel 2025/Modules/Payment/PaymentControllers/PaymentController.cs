

using Management_Hotel_2025.Modules.ManagementQRCode;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Mydata.Models;
using QRCoder;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Payment.PaymentControllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _iconfig;
        private readonly IVnPayService _vnPayService;
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _notifications;
        private readonly ILogger<PaymentController> _logger;
        private readonly IGanarateQRCode _QRcode;
        private readonly EmailProducer _rabbitMQ;

        public PaymentController(IConfiguration configuration, EmailProducer rabbitMQServices, IVnPayService vnPayService, ManagermentHotelContext managermentHotelContext, INotifications notifications, ILogger<PaymentController> logger, IGanarateQRCode qRCode)
        {
            _iconfig = configuration;
            _vnPayService = vnPayService;
            _dbcontext = managermentHotelContext;
            _notifications = notifications;
            _logger = logger;
            _QRcode = qRCode;
            _rabbitMQ = rabbitMQServices;
        }

        [HttpPost]

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            var CustomterName = Request.Form["CustomerName"];
            var CustomterPhone = Request.Form["PhoneNumber"];
            var Email = Request.Form["Email"];
            var Nationality = Request.Form["Nationality"];


            HttpContext.Session.SetString("CustomerName", CustomterName);
            HttpContext.Session.SetString("CustomerPhone", CustomterPhone);
            HttpContext.Session.SetString("Email", Email);
            HttpContext.Session.SetString("Nationality", Nationality);
            return Redirect(url);
        }




        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            string codeHotel = "TDH";

            PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);

            var IdUser = User.FindFirst("IdUser")?.Value;
            var Id = Convert.ToInt32(IdUser);

            // id phòng đang booking
            int? IdRoom = HttpContext.Session.GetInt32("IdRoom");


            decimal TotalRoom = Convert.ToDecimal(HttpContext.Session.GetString("TotalRoom"));

            decimal DepositAmount = Convert.ToDecimal(HttpContext.Session.GetString("DepositAmount"));


            var CustomerName = HttpContext.Session.GetString("CustomerName");
            var CustomerPhone = HttpContext.Session.GetString("CustomerPhone");
            var Nationality = HttpContext.Session.GetString("Nationality");
            var Email = HttpContext.Session.GetString("Email");
            // thánh toán thành công 

            using var transaction = await _dbcontext.Database.BeginTransactionAsync();              // transaction 
            try
            {
                if (response.Success)
                {


                    string? OldCodeBooking = _dbcontext.Bookings
                        .OrderByDescending(s => s.BookingCode)
                        .Select(s => s.BookingCode)
                        .FirstOrDefault();

                    if (string.IsNullOrEmpty(OldCodeBooking))
                    {
                        OldCodeBooking = "TDH000001";
                    }

                    // lấy số  nguyên cộng  thêm 1 , bỏ 3 ký tự đầu
                    int Code = int.Parse(OldCodeBooking.Substring(3)) + 1;

                    // chuyuern 
                    string CodeBookingCode = codeHotel + Code.ToString("D6");


                    var NewBooking = new Booking
                    {
                        BookingDate = DateTime.Now,
                        BookingSource = Id == 0 ? "Walk" : "Website",
                        DepositAmount = DepositAmount,
                        TotalAmountBooking = TotalRoom,
                        Status = "Success",
                        CustomerName = CustomerName,
                        CustomerPhone = CustomerPhone,
                        Nationality = Nationality,
                        Email = Email,
                        BookingCode = CodeBookingCode
                    };
                    // check xem có id thằng user không thì mới gán
                    if (Id != 0)
                    {
                        NewBooking.UserId = Id;
                    }
                    // lưu vào để chuyển qua  bên email
                    HttpContext.Session.SetString("CodeBooking", CodeBookingCode);

                    _dbcontext.Bookings.Add(NewBooking);


                    await _dbcontext.SaveChangesAsync();

                    int idBooking = NewBooking.BookingId;  // booking id  vừa tạo xong


                    var NewbookingDetail = new BookingDetail
                    {
                        BookingId = idBooking,
                        RoomId = IdRoom.Value,
                        CheckInDate = Convert.ToDateTime(HttpContext.Session.GetString("StartDate")),
                        CheckOutDate = Convert.ToDateTime(HttpContext.Session.GetString("EndDate")),
                    };

                    _dbcontext.BookingDetails.Add(NewbookingDetail);

                    await _dbcontext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return RedirectToAction("ResultPayment", "Payment", new { success = true });
                }
                else
                {
                    await transaction.RollbackAsync();
                    // thành toán thất bại
                    return BadRequest("Payment failed. Please try again.");
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        [HttpPost]
        public IActionResult InformationBooking(string NameRoom, decimal Amount, int IdRoom)
        {

            string testmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation($"mail ở info  :{testmail}");

            HttpContext.Session.SetString("NameRoom", NameRoom);
            HttpContext.Session.SetString("Amount", Amount.ToString());
            HttpContext.Session.SetInt32("IdRoom", IdRoom);

            ViewBag.idRoom = IdRoom;

            // số ngày  ở của hành khách
            DateTime ExpectedChechInTime = Convert.ToDateTime(HttpContext.Session.GetString("StartDate"));
            DateTime ExpectedCheckOutTime = Convert.ToDateTime(HttpContext.Session.GetString("EndDate"));

            TimeSpan NumberDate = ExpectedCheckOutTime - ExpectedChechInTime;

            int Days = NumberDate.Days;
            // giá phòng  
            decimal PriceRoom = Convert.ToDecimal(HttpContext.Session.GetString("Amount"));

            //tổng số tiền phòng 

            decimal TotalRoom = Days * PriceRoom;

            //số tiền cọc
            decimal DepositAmount = (Days * PriceRoom) * 0.2m;



            //
            HttpContext.Session.SetString("DepositAmount", DepositAmount.ToString());
            HttpContext.Session.SetString("TotalRoom", TotalRoom.ToString());
            HttpContext.Session.SetString("TotalDays", Days.ToString());
            return View();
        }

        public async Task<IActionResult> ResultPayment()
        {
            string deposit = HttpContext.Session.GetString("DepositAmount");
            string formatted = "";
            if (decimal.TryParse(deposit, out decimal depositAmount))
            {
                formatted = depositAmount.ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
                // formatted = "50.000 ₫"
            }



            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? HttpContext.Session.GetString("Email");
            var name = User.FindFirst("FullName")?.Value ?? HttpContext.Session.GetString("CustomerName");
            var phone = User.FindFirst("PhoneNumber")?.Value ?? HttpContext.Session.GetString("CustomerPhone");
            var roomType = HttpContext.Session.GetString("NameRoom");
            var checkIn = HttpContext.Session.GetString("StartDate");
            var checkOut = HttpContext.Session.GetString("EndDate");
            var guestCount = HttpContext.Session.GetString("GuestCount");
            var totalPrice = HttpContext.Session.GetString("TotalRoom");

            //  booking code 
            var BookingCode = HttpContext.Session.GetString("CodeBooking");


            // tạo qr code từ booking code
            var QRBookingCode = _QRcode.GenerateQRCodeForBookingDetail(BookingCode);
            //var QRBookingCode = "Test";
            string Content = $@"
<p>Kính gửi Quý khách,</p>

<p>Chúng tôi xin trân trọng cảm ơn Quý khách đã tin tưởng và lựa chọn dịch vụ của 
<b>Khách sạn Luxury Trung Đức</b>.</p>

<p>Chúng tôi xin thông báo rằng việc đặt phòng của Quý khách đã <b>THÀNH CÔNG</b> với các thông tin sau:</p>

<ul>
  <li><b>Họ và tên:</b> {name}</li>
  <li><b>Số điện thoại:</b> {phone}</li>
  <li><b>Email:</b> {email}</li>
  <li><b>Loại phòng:</b> {roomType}</li>
  <li><b>Ngày nhận phòng:</b> {checkIn}</li>
  <li><b>Ngày trả phòng:</b> {checkOut}</li>
  <li><b>Số lượng khách:</b> {guestCount}</li>
  <li><b>Số tiền đã đặt cọc:</b> {formatted}</li>
</ul>

<p>Quý khách vui lòng có mặt tại khách sạn vào ngày nhận phòng và mang theo giấy tờ tùy thân để hoàn tất thủ tục check-in.</p>
<p>Để check-in nhanh chóng, Quý khách vui lòng đưa mã QR bên dưới cho bộ phận Tiếp tân.</p>

";





            // đẩy message vào rabbitmq
            await _rabbitMQ.SendMessages(new RabbitMQMessages()
            {
                To = email,
                Subject = "Xác nhận đặt phòng thành công - Khách sạn Luxury Trung Đức",
                Body = Content,
                QRcode = QRBookingCode,
                Type = _iconfig["Status:BookingSuccess"] ?? "BookingSuccess"

            });


            return View();
        }


    }
}
