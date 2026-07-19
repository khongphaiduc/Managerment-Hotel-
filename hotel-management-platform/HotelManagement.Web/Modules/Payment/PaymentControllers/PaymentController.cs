

using Management_Hotel_2025.Modules.ManagementQRCode;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using Management_Hotel_2025.Modules.RedisServices;
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

        // Creates a VNPay payment URL, stores booking data in session, and acquires a temporary room lock.
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model, [FromServices] IRedisLockService _redisLock)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            var CustomterName = Request.Form["CustomerName"];
            var CustomterPhone = Request.Form["PhoneNumber"];
            var Email = Request.Form["Email"];
            var Nationality = Request.Form["Nationality"];
            int? IdRoom = HttpContext.Session.GetInt32("IdRoom");

            HttpContext.Session.SetString("CustomerName", CustomterName);
            HttpContext.Session.SetString("CustomerPhone", CustomterPhone);
            HttpContext.Session.SetString("Email", Email);
            HttpContext.Session.SetString("Nationality", Nationality);



            var LockKey = IdRoom.ToString() ?? "None";
            var LockVlaue = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("LockKey", LockKey);
            HttpContext.Session.SetString("LockValue", LockVlaue);

            var result = await _redisLock.AcquireAsync(LockKey, LockVlaue, TimeSpan.FromMinutes(3));

            if (!result)
            {
                TempData["Error"] = "Ô ồ bạn chậm tay mất rồi , phòng đang được người khách thanh toán . Vui lòng thử lại sau";
            }

            return Redirect(url);
        }




        [HttpGet]
        // Verifies the VNPay callback and creates the booking and booking detail when payment succeeds.
        public async Task<IActionResult> PaymentCallbackVnpay([FromServices] IRedisLockService _redisLock)
        {
            string codeHotel = "TDH";

            PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);

            var IdUser = User.FindFirst("IdUser")?.Value;
            var Id = Convert.ToInt32(IdUser);

            int? IdRoom = HttpContext.Session.GetInt32("IdRoom");


            decimal TotalRoom = Convert.ToDecimal(HttpContext.Session.GetString("TotalRoom"));

            decimal DepositAmount = Convert.ToDecimal(HttpContext.Session.GetString("DepositAmount"));


            var CustomerName = HttpContext.Session.GetString("CustomerName");
            var CustomerPhone = HttpContext.Session.GetString("CustomerPhone");
            var Nationality = HttpContext.Session.GetString("Nationality");
            var Email = HttpContext.Session.GetString("Email");

            var LockKey = HttpContext.Session.GetString("LockKey");
            var LockValue = HttpContext.Session.GetString("LockValue");


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

                    int Code = int.Parse(OldCodeBooking.Substring(3)) + 1;

                    // Build the next booking code using the hotel prefix and a six-digit sequence.
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
                    if (Id != 0)
                    {
                        NewBooking.UserId = Id;
                    }
                    HttpContext.Session.SetString("CodeBooking", CodeBookingCode);

                    _dbcontext.Bookings.Add(NewBooking);


                    await _dbcontext.SaveChangesAsync();

                    var idBooking = NewBooking.BookingId;



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
                    await _redisLock.ReleaseAsync(LockKey, LockValue);

                    return BadRequest("Payment failed. Please try again.");
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                await _redisLock.ReleaseAsync(LockKey, LockValue);
                throw;
            }

        }

        [HttpPost]
        // Stores booking and payment information submitted before redirecting to the payment flow.
        public IActionResult InformationBooking(string NameRoom, decimal Amount, int IdRoom)
        {

            string testmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation($"mail ở info  :{testmail}");

            HttpContext.Session.SetString("NameRoom", NameRoom);
            HttpContext.Session.SetString("Amount", Amount.ToString());
            HttpContext.Session.SetInt32("IdRoom", IdRoom);

            ViewBag.idRoom = IdRoom;

            DateTime ExpectedChechInTime = Convert.ToDateTime(HttpContext.Session.GetString("StartDate"));
            DateTime ExpectedCheckOutTime = Convert.ToDateTime(HttpContext.Session.GetString("EndDate"));

            TimeSpan NumberDate = ExpectedCheckOutTime - ExpectedChechInTime;

            int Days = NumberDate.Days;
            decimal PriceRoom = Convert.ToDecimal(HttpContext.Session.GetString("Amount"));


            decimal TotalRoom = Days * PriceRoom;

            decimal DepositAmount = (Days * PriceRoom) * 0.2m;



            //
            HttpContext.Session.SetString("DepositAmount", DepositAmount.ToString());
            HttpContext.Session.SetString("TotalRoom", TotalRoom.ToString());
            HttpContext.Session.SetString("TotalDays", Days.ToString());
            return View();
        }

        // Displays the final payment result and related booking information.
        public async Task<IActionResult> ResultPayment()
        {
            string deposit = HttpContext.Session.GetString("DepositAmount");
            string formatted = "";
            if (decimal.TryParse(deposit, out decimal depositAmount))
            {
                formatted = depositAmount.ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
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
