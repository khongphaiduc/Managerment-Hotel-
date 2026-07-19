using API_BookingHotel.Modules.Invoice.MInvoiceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API_BookingHotel.Modules.Invoice.InvoicePassengerControllers
{
    [Authorize(Roles ="Admin,Staff")]
    [Route("admin")]
    [ApiController]
    public class MInvoiceController : ControllerBase
    {
        private readonly IInvoiceServices _Invoices;

        public MInvoiceController(IInvoiceServices invoiceServices)
        {
            _Invoices = invoiceServices;

        }

        [HttpGet("invoice")]
// Returns a paginated invoice list filtered by keyword and optional date range.
        public async Task<IActionResult> GetInvoicePassenger(string? key, DateTime? startdate, DateTime? enddate, int indexpage)
        {

            var result = await _Invoices.GetListInvoicePasseners(key, startdate, enddate, indexpage);

            if (result == null)
            {
                return NotFound("Không tìm thấy hóa đơn");
            }
            else
            {
                return Ok(result);
            }
        }


        [HttpGet("invoice/{invoiceCode}")]
// Returns the invoice matching the supplied invoice code.
        public async Task<IActionResult> GetInvoicePassengerByCode(string invoiceCode)
        {
            var result = await _Invoices.GetInvoicePassengerByCode(invoiceCode);
            if (result == null)
            {
                return NotFound("Không tìm thấy hóa đơn");
            }
            else
            {
                return Ok(result);
            }

        }
    }
}
