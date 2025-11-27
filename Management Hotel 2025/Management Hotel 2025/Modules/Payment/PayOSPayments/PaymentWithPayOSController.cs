
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
        private readonly IGameBlackRed _blackred;
        private readonly IHubContext<CoinHub> _coinHub;
        private readonly IOrder _iorder;
        private readonly IHubContext<NotificationSystem> _hubNotificationSystem;
        private readonly ManagermentHotelContext _dbcontext;

        public PaymentWithPayOSController(PayOSClient payOS, ILogger<PaymentWithPayOSController> logger, IGameBlackRed gameBlackRed, IHubContext<CoinHub> coinHub, IOrder order, IHubContext<NotificationSystem> hubNotificationSystem, ManagermentHotelContext managermentHotelContext)
        {
            _payOS = payOS;
            _logger = logger;
            _blackred = gameBlackRed;
            _coinHub = coinHub;
            _iorder = order;
            _hubNotificationSystem = hubNotificationSystem;
            _dbcontext = managermentHotelContext;

        }



        // nạp xu 
        [HttpPost("createpayos")]
        public async Task<IActionResult> createpayos([FromBody] PaymentRequest payment)
        {
            try
            {
                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(DateTime.Now.ToString("MMddHHmmss") + (User.FindFirst("IdUser")?.Value ?? "00")),
                    Amount = payment.Amount,
                    Description = "NAPXU",
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
                    return Ok("Tạo thanh toán thất bại");
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

        // thanht toán chuyền khoản (check out) 
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




        // webhook nhận thông báo từ ngân hàng 
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Webhook webhook)
        {
            // Verify webhook từ PayOS
            var webhookData = await _payOS.Webhooks.VerifyAsync(webhook);
            var pay_identity = HttpContext.Session.GetString("Pay_Identity");
            if (webhookData.Code == "00") // thanh toán thành công
            {
                // Lấy thông tin thanh toán
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
                // ------------------------------------------------------------------------------------ thanh toán  hóa đơn check out phòng ------------------------------------------------------------------------------------

                if (webhookData.Description != "NAPXU" && !webhookData.Description.StartsWith("PTDHOTEL"))
                {
                    Console.WriteLine("Round 1 ");  // debug
                    // webhookData.Description bằng với BookingCode

                    var resutl1 = await _iorder.ConfirmTranfersQRcode(webhookData.Description);
                    await _hubNotificationSystem.Clients.All.SendAsync("NotificationTransferQRCode", $"Thanh toán chuyển khoản thành công cho mã đặt phòng {webhookData.Description} với số tiền {amount} VND");
                    return Ok();
                    //------------------------------------------------------------------------------------  thành toán booking bằng payos ------------------------------------------------------------------------------------
                }
                else if (webhookData.Description.StartsWith("PTDHOTEL"))
                {
                    var idUser = webhookData.Description.Substring(13);  // lấy id của user từ Description trong payos

                    Console.WriteLine("Round 2 ");
                    string codeHotel = "TDH";

                    // note toàn bộ các data lấy từ session sẽ bị null bởi vì nó webhook nó không nằm cùng context của user

                    var bookingTemporary = await _dbcontext.BookingTemporaryPayOs.FirstOrDefaultAsync(s => s.PaymentCode == webhookData.Description);

                    if (bookingTemporary == null) return BadRequest();

                    string? OldCodeBooking = _dbcontext.Bookings
                   .OrderByDescending(s => s.BookingCode)
                   .Select(s => s.BookingCode)
                   .FirstOrDefault();

                    // lấy số  nguyên cộng  thêm 1 , bỏ 3 ký tự đầu
                    int Code = int.Parse(OldCodeBooking.Substring(3)) + 1;

                    // chuyuern 
                    string CodeBookingCode = codeHotel + Code.ToString("D6");



                    var NewBooking = new Booking
                    {
                        BookingDate = DateTime.Now,
                        BookingSource = "Website",
                        DepositAmount = bookingTemporary.DepositAmount ?? 000,
                        TotalAmountBooking = bookingTemporary.NumberOFRoom ?? 00,
                        Status = "Success",
                        CustomerName = bookingTemporary.NameCustomer ?? "Vô danh",
                        CustomerPhone = bookingTemporary.PhoneNumber ?? "000000",
                        Nationality = bookingTemporary.Nationality ?? "00000",
                        Email = bookingTemporary.Email ?? "0000",
                        BookingCode = CodeBookingCode
                    };
                    // check xem có id thằng user không thì mới gán
                    if (Int32.Parse(idUser) != 0)
                    {
                        NewBooking.UserId = Int32.Parse(idUser);
                    }
                  
                    _dbcontext.Bookings.Add(NewBooking);
                    _dbcontext.SaveChanges();
                    int idBooking = NewBooking.BookingId;  // booking id  vừa tạo xong

                    var NewbookingDetail = new BookingDetail
                    {
                        BookingId = idBooking,
                        RoomId = bookingTemporary.IdRoom ?? 00,
                        CheckInDate = bookingTemporary.StartDate,
                        CheckOutDate = bookingTemporary.EndDate,
                    };

                    _dbcontext.BookingDetails.Add(NewbookingDetail);
                    var row = _dbcontext.SaveChanges();


                    Console.WriteLine($"Giá trị của Row là {row}");
                    if (row > 0)
                    {
                        Console.WriteLine($"Giá trị của Row là {row}");


                        await _hubNotificationSystem.Clients.User(idUser).SendAsync("NotificationBookingByPayOS", CodeBookingCode, bookingTemporary.DepositAmount);


                        return Ok();
                    }
                    else
                    {
                        Console.WriteLine("Đặt phòng thất bại. Vui lòng thử lại.");
                        return BadRequest("Đặt phòng thất bại. Vui lòng thử lại.");
                    }

                }
                else
                {

                    Console.WriteLine("Round 3 ");

                    //------------------------------------------------------------------------------------  nạp thẻ ------------------------------------------------------------------------------------
                    int idUser = int.Parse(orderCode.ToString().Substring(10));

                    var result = await _blackred.AddCoinForUsers(idUser, amount);

                    if (result)
                    {
                        // sử dung SignalR để thông báo user nạp tiền thành công 

                        Console.WriteLine("IDUSer là " + idUser);



                        await _coinHub.Clients.User(idUser.ToString())
                           .SendAsync("ReceiveCoinUpdate", amount, "Nạp tiền thành công!");

                        var totalCoin = await _blackred.GetCoinsForUsers(idUser);

                        _logger.LogInformation("Tổng số coin user đang có là " + totalCoin);

                        await _coinHub.Clients.User(idUser.ToString())
                           .SendAsync("UpdateCoin", totalCoin);


                        Console.WriteLine($"Nạp xu thành công cho user ID: {idUser} với số xu: {amount}");
                    }
                    else
                    {

                        Console.WriteLine($"Nạp xu thất bại cho user ID: {idUser}");
                    }

                    return Ok(webhook);
                }
            }
            else
            {

                Console.WriteLine($"Thanh toán thất bại: {webhookData.Code} - {webhookData.Description2}");
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



