

namespace Management_Hotel_2025.Modules.Invoices.InvocieModels
{
    public class InvoicesViewModel
    {
      
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public string RoomNumber { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string StatusInvoice { get; set; }
        public string? CreatedBy { get; set; }
        // 


        public string BookingCode { get; set; }


    }
}
