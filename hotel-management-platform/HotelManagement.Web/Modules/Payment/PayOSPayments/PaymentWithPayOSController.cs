
using Management_Hotel_2025.Modules.ManagementQRCode;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using Management_Hotel_2025.Modules.Rooms.RoomService;
using Management_Hotel_2025.Modules.SignalRModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Mydata.Models;
using PayOS;
using PayOS.Exceptions;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using QRCoder;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Management_Hotel_2025.Modules.Payment.PayOSPayments
{
    [Route("payos")]
    [ApiController]
    public class PaymentWithPayOSController : ControllerBase
    {
        private readonly PayOSClient _payOS;
        private readonly ILogger<PaymentWithPayOSController> _logger;
        private readonly IOrder _iorder;
        private readonly IHubContext<NotificationSystem> _hubNotificationSystem;
        private readonly ManagermentHotelContext _dbcontext;
        private readonly INotifications _iNotification;
        private readonly IGanarateQRCode _QRcode;
        private readonly IConfiguration _iconfig;
        private readonly EmailProducer _rabbitMQ;

        public PaymentWithPayOSController(EmailProducer rabbitMQServices, IConfiguration configuration, PayOSClient payOS, ILogger<PaymentWithPayOSController> logger, IOrder order, IHubContext<NotificationSystem> hubNotificationSystem, ManagermentHotelContext managermentHotelContext, INotifications notifications, IGanarateQRCode ganarateQRCode)
        {
            _payOS = payOS;
            _logger = logger;
            _iorder = order;
            _hubNotificationSystem = hubNotificationSystem;
            _dbcontext = managermentHotelContext;
            _iNotification = notifications;
            _QRcode = ganarateQRCode;
            _iconfig = configuration;
            _rabbitMQ = rabbitMQServices;

        }



        [HttpPost("checkoutbypayos")]
        public async Task<IActionResult> createpayosCheckOut([FromBody] PaymentRequest payment)
        {
            try
            {
                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
                    Amount = payment.Amount,
                    Description = payment.Description,
                    ReturnUrl = "https://your-url.com",
                    CancelUrl = "https://your-url.com"
                };

                var paymentLink = await _payOS.PaymentRequests.CreateAsync(paymentRequest);

                _logger.LogInformation(paymentLink.QrCode);

                if (paymentLink.QrCode != null)
                {
                    return Ok(new { qrCode = paymentLink.QrCode });
                }
                else
                {
                    return NotFound("TÃ¡ÂºÂ¡o thanh toÃƒÂ¡n thÃ¡ÂºÂ¥t bÃ¡ÂºÂ¡i");
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




        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Webhook webhook)
        {
            var webhookData = await _payOS.Webhooks.VerifyAsync(webhook);
            var pay_identity = HttpContext.Session.GetString("Pay_Identity");
            if (webhookData.Code == "00")
            {
                var orderCode = webhookData.OrderCode;
                var amount = webhookData.Amount;
                var reference = webhookData.Reference;
                var transactionTime = webhookData.TransactionDateTime;

                Console.WriteLine($"Charged succes");
                Console.WriteLine($"code order: {orderCode}");
                Console.WriteLine($"amunt: {amount}");
                Console.WriteLine($"code reference: {reference}");
                Console.WriteLine($"Time: {transactionTime}");
                Console.WriteLine($"Description: {webhookData.Description}");

                if (!webhookData.Description.StartsWith("PTDHOTEL"))
                {
                    Console.WriteLine("Round 1 ");  // debug

                    var resutl1 = await _iorder.ConfirmTranfersQRcode(webhookData.Description);
                    await _hubNotificationSystem.Clients.All.SendAsync("NotificationTransferQRCode", $"Thanh toÃƒÂ¡n chuyÃ¡Â»Æ’n khoÃ¡ÂºÂ£n thÃƒÂ nh cÃƒÂ´ng cho mÃƒÂ£ Ã„â€˜Ã¡ÂºÂ·t phÃƒÂ²ng {webhookData.Description} vÃ¡Â»â€ºi sÃ¡Â»â€˜ tiÃ¡Â»Ân {amount} VND");
                    return Ok();
                }
                else if (webhookData.Description.StartsWith("PTDHOTEL"))
                {
                    var idUser = webhookData.Description.Substring(13);

                    Console.WriteLine("Round 2 ");



                    var bookingTemporary = await _dbcontext.BookingTemporaryPayOs.FirstOrDefaultAsync(s => s.PaymentCode == webhookData.Description);

                    if (bookingTemporary == null) return BadRequest();

                    string? OldCodeBooking = _dbcontext.Bookings
                   .OrderByDescending(s => s.BookingCode)
                   .Select(s => s.BookingCode)
                   .FirstOrDefault();




                    var NewBooking = new Booking
                    {
                        BookingDate = DateTime.Now,
                        BookingSource = "Website",
                        DepositAmount = bookingTemporary.DepositAmount ?? 000,
                        TotalAmountBooking = bookingTemporary.NumberOFRoom ?? 00,
                        Status = "Success",
                        CustomerName = bookingTemporary.NameCustomer ?? "VÃƒÂ´ danh",
                        CustomerPhone = bookingTemporary.PhoneNumber ?? "000000",
                        Nationality = bookingTemporary.Nationality ?? "00000",
                        Email = bookingTemporary.Email ?? "0000",
                        BookingCode = bookingTemporary.BookingCode
                    };
                    if (Int32.Parse(idUser) != 0)
                    {
                        NewBooking.UserId = Int32.Parse(idUser);
                    }

                    _dbcontext.Bookings.Add(NewBooking);
                    _dbcontext.SaveChanges();
                    int idBooking = NewBooking.BookingId;

                    var NewbookingDetail = new BookingDetail
                    {
                        BookingId = idBooking,
                        RoomId = bookingTemporary.IdRoom ?? 00,
                        CheckInDate = bookingTemporary.StartDate,
                        CheckOutDate = bookingTemporary.EndDate,
                    };

                    _dbcontext.BookingDetails.Add(NewbookingDetail);
                    var row = _dbcontext.SaveChanges();



                    Console.WriteLine($"GiÃƒÂ¡ trÃ¡Â»â€¹ cÃ¡Â»Â§a Row lÃƒÂ  {row}");
                    if (row > 0)
                    {
                        Console.WriteLine($"GiÃƒÂ¡ trÃ¡Â»â€¹ cÃ¡Â»Â§a Row lÃƒÂ  {row}");


                        await _hubNotificationSystem.Clients.User(idUser).SendAsync("NotificationBookingByPayOS", bookingTemporary.BookingCode, bookingTemporary.DepositAmount);

                        var QRBookingCode = _QRcode.GenerateQRCodeForBookingDetail(bookingTemporary.BookingCode);   // qr code 

                        string Content = $@"
<p>KÃƒÂ­nh gÃ¡Â»Â­i QuÃƒÂ½ khÃƒÂ¡ch,</p>

<p>ChÃƒÂºng tÃƒÂ´i xin trÃƒÂ¢n trÃ¡Â»Âng cÃ¡ÂºÂ£m Ã†Â¡n QuÃƒÂ½ khÃƒÂ¡ch Ã„â€˜ÃƒÂ£ tin tÃ†Â°Ã¡Â»Å¸ng vÃƒÂ  lÃ¡Â»Â±a chÃ¡Â»Ân dÃ¡Â»â€¹ch vÃ¡Â»Â¥ cÃ¡Â»Â§a 
<b>KhÃƒÂ¡ch sÃ¡ÂºÂ¡n Luxury Trung Ã„ÂÃ¡Â»Â©c</b>.</p>

<p>ChÃƒÂºng tÃƒÂ´i xin thÃƒÂ´ng bÃƒÂ¡o rÃ¡ÂºÂ±ng viÃ¡Â»â€¡c Ã„â€˜Ã¡ÂºÂ·t phÃƒÂ²ng cÃ¡Â»Â§a QuÃƒÂ½ khÃƒÂ¡ch Ã„â€˜ÃƒÂ£ <b>THÃƒâ‚¬NH CÃƒâ€NG</b> vÃ¡Â»â€ºi cÃƒÂ¡c thÃƒÂ´ng tin sau:</p>

<ul>
  <li><b>HÃ¡Â»Â vÃƒÂ  tÃƒÂªn:</b> {bookingTemporary.NameCustomer}</li>
  <li><b>SÃ¡Â»â€˜ Ã„â€˜iÃ¡Â»â€¡n thoÃ¡ÂºÂ¡i:</b> {bookingTemporary.PhoneNumber}</li>
  <li><b>Email:</b> {bookingTemporary.Email}</li>
  <li><b>NgÃƒÂ y nhÃ¡ÂºÂ­n phÃƒÂ²ng:</b> {bookingTemporary.StartDate}</li>
  <li><b>NgÃƒÂ y trÃ¡ÂºÂ£ phÃƒÂ²ng:</b> {bookingTemporary.EndDate}</li>
  <li><b>SÃ¡Â»â€˜ tiÃ¡Â»Ân Ã„â€˜ÃƒÂ£ Ã„â€˜Ã¡ÂºÂ·t cÃ¡Â»Âc:</b> {bookingTemporary.DepositAmount}</li>
</ul>

<p>QuÃƒÂ½ khÃƒÂ¡ch vui lÃƒÂ²ng cÃƒÂ³ mÃ¡ÂºÂ·t tÃ¡ÂºÂ¡i khÃƒÂ¡ch sÃ¡ÂºÂ¡n vÃƒÂ o ngÃƒÂ y nhÃ¡ÂºÂ­n phÃƒÂ²ng vÃƒÂ  mang theo giÃ¡ÂºÂ¥y tÃ¡Â»Â tÃƒÂ¹y thÃƒÂ¢n Ã„â€˜Ã¡Â»Æ’ hoÃƒÂ n tÃ¡ÂºÂ¥t thÃ¡Â»Â§ tÃ¡Â»Â¥c check-in.</p>
<p>Ã„ÂÃ¡Â»Æ’ check-in nhanh chÃƒÂ³ng, QuÃƒÂ½ khÃƒÂ¡ch vui lÃƒÂ²ng Ã„â€˜Ã†Â°a mÃƒÂ£ QR bÃƒÂªn dÃ†Â°Ã¡Â»â€ºi cho bÃ¡Â»â„¢ phÃ¡ÂºÂ­n TiÃ¡ÂºÂ¿p tÃƒÂ¢n.</p>

";

                        await _rabbitMQ.SendMessages(new RabbitMQMessages()
                        {
                            To = bookingTemporary.Email,
                            Subject = "XÃƒÂ¡c nhÃ¡ÂºÂ­n Ã„â€˜Ã¡ÂºÂ·t phÃƒÂ²ng thÃƒÂ nh cÃƒÂ´ng - KhÃƒÂ¡ch sÃ¡ÂºÂ¡n Luxury Trung Ã„ÂÃ¡Â»Â©c",
                            Body = Content,
                            QRcode = QRBookingCode,
                            Type = _iconfig["Status:BookingSuccess"]!

                        });

                        return Ok();
                    }
                    else
                    {
                        Console.WriteLine("Ã„ÂÃ¡ÂºÂ·t phÃƒÂ²ng thÃ¡ÂºÂ¥t bÃ¡ÂºÂ¡i. Vui lÃƒÂ²ng thÃ¡Â»Â­ lÃ¡ÂºÂ¡i.");
                        return BadRequest("Ã„ÂÃ¡ÂºÂ·t phÃƒÂ²ng thÃ¡ÂºÂ¥t bÃ¡ÂºÂ¡i. Vui lÃƒÂ²ng thÃ¡Â»Â­ lÃ¡ÂºÂ¡i.");
                    }

                }
                else
                {
                    return BadRequest("Top-up payments are no longer supported.");
                }
            }
            else
            {

                Console.WriteLine($"Thanh toÃƒÂ¡n thÃ¡ÂºÂ¥t bÃ¡ÂºÂ¡i: {webhookData.Code} - {webhookData.Description2}");
                return BadRequest();
            }
        }
    }





    public class PaymentRequest
    {
        public long Amount { get; set; }

        public string Description { get; set; }
    }
}



