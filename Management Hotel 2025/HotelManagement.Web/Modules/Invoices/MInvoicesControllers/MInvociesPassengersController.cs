using Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers;
using Management_Hotel_2025.Modules.Invoices.InvocieModels;
using Management_Hotel_2025.Modules.Rooms.RoomService;
using ApiInvoices = API_BookingHotel.Modules.Invoice.MInvoiceServices;
using Microsoft.AspNetCore.Mvc;
using Mydata.Models;
using System.Net.Http.Headers;



namespace Management_Hotel_2025.Modules.Invoices.MInvoicesControllers
{
    [Route("admin")]
    public class MInvociesPassengersController : Controller
    {
        private readonly ILogger<MInvociesPassengersController> _Ilogger;
        private readonly IOrder _iOrder;
        private readonly ApiInvoices.IInvoiceServices _invoiceService;

        public MInvociesPassengersController(ILogger<MInvociesPassengersController> logger, IOrder order, ApiInvoices.IInvoiceServices invoiceService)
        {
            _Ilogger = logger;
            _iOrder = order;
            _invoiceService = invoiceService;
        }




        [HttpGet("invoices")]
        // Handles the GetlistInvoicesPassengers action.
        public async Task<IActionResult> GetlistInvoicesPassengers(string? key, DateTime? startdate, DateTime? enddate, int indexpage = 1)
        {
            try
            {

                if (!startdate.HasValue) startdate = DateTime.Now.AddDays(-7);

                if (!enddate.HasValue) enddate = DateTime.Now;


                var result = await _invoiceService.GetListInvoicePasseners(key, startdate, enddate, indexpage);
                ViewBag.Key = key;
                ViewBag.PageIndex = result.PageIndex;
                ViewBag.TotalPages = result.TotalPages;
                ViewBag.CurrentSearchKey = key;
                ViewBag.CurrentStartDate = startdate?.ToString("yyyy-MM-dd");
                ViewBag.CurrentEndDate = enddate?.ToString("yyyy-MM-dd");

                var items = result.Items.Select(item => new InvoicesViewModel
                {
                    BookingCode = item.BookingCode,
                    InvoiceCode = item.InvoiceCode,
                    CustomerName = item.CustomerName,
                    RoomNumber = item.RoomNumber,
                    CheckInDate = item.CheckInDate,
                    CheckOutDate = item.CheckOutDate,
                    TotalAmount = item.TotalAmount,
                    StatusInvoice = item.StatusInvoice,
                    CreatedBy = item.CreatedBy
                }).ToList();

                ViewBag.TotalAmount = items.Sum(s => s.TotalAmount);
                return View(items);
            }
            catch (Exception s)
            {
                _Ilogger.LogInformation($"Bug : {s.Message}");
                throw;
            }
        }


        [HttpGet("invoices/{bookingcode}")]
        // Handles the DetailInvoicesPassenger action.
        public async Task<IActionResult> DetailInvoicesPassenger(string bookingcode)
        {
            var order = await _iOrder.ViewOrder(bookingcode);
            ViewBag.TimeCheckOut = order.RealCheckOutDate;
            return View(order);
        }

    }
}
