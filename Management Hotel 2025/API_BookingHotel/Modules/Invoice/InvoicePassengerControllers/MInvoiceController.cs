using API_BookingHotel.Modules.Invoice.MInvoiceServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API_BookingHotel.Modules.Invoice.InvoicePassengerControllers
{
    [Route("admin")]
    [ApiController]
    public class MInvoiceController : ControllerBase
    {
        private readonly IInvoiceServices _Invoices;

        public MInvoiceController(IInvoiceServices invoiceServices)
        {
            _Invoices = invoiceServices;

        }

        // api lấy danh sách hóa đơn
        [HttpGet("invoice")]
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


        // api lấy hóa đơn theo mã
        [HttpGet("invoice/{invoiceCode}")]
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
