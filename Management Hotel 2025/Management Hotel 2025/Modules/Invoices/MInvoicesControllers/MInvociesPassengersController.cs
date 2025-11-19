using Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers;
using Management_Hotel_2025.Modules.Invoices.InvocieModels;
using Management_Hotel_2025.Modules.Rooms.RoomService;
using Microsoft.AspNetCore.Mvc;



namespace Management_Hotel_2025.Modules.Invoices.MInvoicesControllers
{
    [Route("admin")]
    public class MInvociesPassengersController : Controller
    {
        private readonly IConfiguration _Iconfig;
        private string apiBaseUrl;
        private readonly ILogger<MInvociesPassengersController> _Ilogger;
        private readonly IOrder _iOrder;

        public MInvociesPassengersController(IConfiguration configuration, ILogger<MInvociesPassengersController> logger, IOrder order)
        {
            _Iconfig = configuration;
            apiBaseUrl = _Iconfig["ApiHotel:PassengerInvoice"];
            _Ilogger = logger;
            _iOrder = order;
        }


        // lấy danh sách hóa đơn
        [HttpGet("invoice")]
        public async Task<IActionResult> GetlistInvoicesPassengers(string? key, DateTime? startdate, DateTime? enddate, int indexpage = 1)
        {
            try
            {

                if (!startdate.HasValue) startdate = DateTime.Now.AddDays(-7);

                if (!enddate.HasValue) enddate = DateTime.Now;


                var apiBaseUrl = _Iconfig["ApiHotel:PassengerInvoice"]
                    + $"?key={key}&startdate={startdate}&enddate={enddate}&indexpage={indexpage}";

                using (var httpclient = new HttpClient())
                {
                    var response = await httpclient.GetAsync(apiBaseUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        // API bây giờ nên trả PagedResult<InvoicesViewModel>
                        var pagedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<PagedResult<InvoicesViewModel>>(data);
                        ViewBag.Key = key;
                        ViewBag.PageIndex = pagedResult.PageIndex;
                        ViewBag.TotalPages = pagedResult.TotalPages;
                        ViewBag.CurrentSearchKey = key;
                        ViewBag.CurrentStartDate = startdate?.ToString("yyyy-MM-dd");
                        ViewBag.CurrentEndDate = enddate?.ToString("yyyy-MM-dd");

                        ViewBag.TotalAmount = pagedResult.Items.Sum(s => s.TotalAmount);


                        return View(pagedResult.Items);
                    }
                    else
                    {
                        return View(new List<InvoicesViewModel>());
                    }
                }
            }
            catch (Exception s)
            {
                _Ilogger.LogInformation($"Bug : {s.Message}");
                throw;
            }
        }


        [HttpGet("invoice/{bookingcode}")]
        public async Task<IActionResult> DetailInvoicesPassenger(string bookingcode)
        {
            var order = await _iOrder.ViewOrder(bookingcode);
            ViewBag.TimeCheckOut = order.RealCheckOutDate;
            return View(order);
        }

    }
}
