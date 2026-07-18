using API_BookingHotel.Modules.Invoice.InvoiceModels;

namespace API_BookingHotel.Modules.Invoice.MInvoiceServices
{
    public interface IInvoiceServices
    {
        public Task<PagedResult<InvoiceViewModel>> GetListInvoicePasseners(string? SearchKey, DateTime? startdate, DateTime? enddate, int indexpage);

        public Task<InvoiceViewModel> GetInvoicePassengerByCode(string invoiceCode);
    }
}
