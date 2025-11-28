using Microsoft.AspNetCore.Mvc;
using Mydata.Models;
using PayOS;
using PayOS.Exceptions;
using PayOS.Models.V2.PaymentRequests;
using System.Drawing;
using System.Security.Claims;

namespace Management_Hotel_2025.Modules.Payment.PayOSPayments
{
    [Route("bookingbypayos")]
    public class BookingByPayOSController : Controller
    {
        private readonly PayOSClient _payOS;
        private readonly ILogger<BookingByPayOSController> _logger;
        private readonly ManagermentHotelContext _dbcontext;

        public BookingByPayOSController(PayOSClient payOSClient, ILogger<BookingByPayOSController> logger, ManagermentHotelContext hotelContext)
        {
            _payOS = payOSClient;
            _logger = logger;
            _dbcontext = hotelContext;
        }

        //  payos qr tạo booking 
        [HttpPost("booking")]
        public async Task<IActionResult> CreateBookingByPayOS()
        {

            var idUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string randomcode = Guid.NewGuid().ToString("N").Substring(0, 5);
            var orderDescription = "PTDHOTEL" + randomcode + idUser;

            // lưu thông tin cần thanh toán vào bẳng tạm trong database
            int? IdRoomTemporary = HttpContext.Session.GetInt32("IdRoom");
            decimal TotalRoomTemporary = Convert.ToDecimal(HttpContext.Session.GetString("TotalRoom"));
            decimal DepositAmountTemporary = Convert.ToDecimal(HttpContext.Session.GetString("DepositAmount"));

            var startDateTemporary = HttpContext.Session.GetString("StartDate");
            var endDateTemporary = HttpContext.Session.GetString("EndDate");
            var CustomerNameTemporary = Request.Form["CustomerName"];
            var CustomerPhoneTemporary = Request.Form["PhoneNumber"];
            var NationalityTemporary = Request.Form["Nationality"];
            var EmailTemporary = Request.Form["Email"];


            string? OldCodeBooking = _dbcontext.Bookings
               .OrderByDescending(s => s.BookingCode)
               .Select(s => s.BookingCode)
               .FirstOrDefault();

            // lấy số  nguyên cộng  thêm 1 , bỏ 3 ký tự đầu
            int Code = int.Parse(OldCodeBooking.Substring(3)) + 1;

            // chuyuern 
            string codeHotel = "TDH";
            string CodeBookingCode = codeHotel + Code.ToString("D6");


            _dbcontext.BookingTemporaryPayOs.Add(new MyData.Models.BookingTemporaryPayOS()
            {
                PaymentCode = orderDescription,
                IdRoom = IdRoomTemporary,
                NumberOFRoom = Convert.ToInt32(TotalRoomTemporary),
                DepositAmount = DepositAmountTemporary,
                NameCustomer = CustomerNameTemporary,
                PhoneNumber = CustomerPhoneTemporary,
                Nationality = NationalityTemporary,
                Email = EmailTemporary,
                StartDate = Convert.ToDateTime(startDateTemporary),
                EndDate = Convert.ToDateTime(endDateTemporary),
                BookingCode = CodeBookingCode
            });

            await _dbcontext.SaveChangesAsync();


            // tao đơn thanh toán payos
            decimal DepositAmount = Convert.ToDecimal(HttpContext.Session.GetString("DepositAmount"));
            try
            {


                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(DateTime.Now.ToString("MMddHHmmss")),
                    Amount = Convert.ToInt64(DepositAmount),
                    Description = orderDescription,
                    ReturnUrl = "https://your-url.com",
                    CancelUrl = "https://your-url.com"
                };

                var paymentLink = await _payOS.PaymentRequests.CreateAsync(paymentRequest);

                _logger.LogInformation(paymentLink.QrCode);

                if (paymentLink.QrCode != null)
                {
                    return Ok(new { success = true, qrCode = paymentLink.QrCode });
                }
                else
                {
                    return NotFound("Tạo thanh toán thất bại");
                }

            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                Console.WriteLine($"Error Code: {ex.ErrorCode}");
            }
            catch (PayOSException ex)
            {
                Console.WriteLine($"PayOS Error: {ex.Message}");
            }


            return Ok("PayOS Payment Gateway is running.");

        }

    }
}
