using Mydata.Models;

namespace Management_Hotel_2025.ViewModel
{
    public class ViewBookingDetail
    {
        public string BookingId { get; set; }

        public string BookingCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BookingStatus { get; set; }
        public string BookingSource { get; set; } // Online, Walk-in, OTA


        public string CustomerFullName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerGender { get; set; }
        public string CustomerNationality { get; set; }
        public string CustomerIdentityNumber { get; set; } // CMND / Passport
        public string CustomerSpecialRequest { get; set; }

        public int NumberOfRoom { get; set; }

        public List<ViewDetailRoom> ListDetailRoom { get; set; } = new List<ViewDetailRoom>();

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int NumberOfNights
        {
            get
            {
                return (CheckOutDate - CheckInDate).Days;
            }
        }



        public decimal TotalAmountRoom { get; set; }
        public decimal DepositAmount { get; set; }

        public decimal Discount { get; set; }

        public string PaymentMethod { get; set; } // Cash, Card, Transfer



        public List<string> ModifiedBy { get; set; } = new List<string>();

        public string QRCode { get; set; }

    }
}
