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

        public BookingByPayOSController(PayOSClient payOSClient, ILogger<BookingByPayOSController> logger)
        {
            _payOS = payOSClient;
            _logger = logger;
        }

        //  payos qr tạo booking 
        [HttpPost("booking")]
        public async Task<IActionResult> CreateBookingByPayOS()
        {
            decimal DepositAmount = Convert.ToDecimal(HttpContext.Session.GetString("DepositAmount"));
            try
            {
                var idUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(DateTime.Now.ToString("MMddHHmmss")),
                    Amount = Convert.ToInt64(DepositAmount),
                    Description = "PTDHOTEL" + idUser,
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
