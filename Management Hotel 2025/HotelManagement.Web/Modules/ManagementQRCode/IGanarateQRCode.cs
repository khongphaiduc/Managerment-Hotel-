namespace Management_Hotel_2025.Modules.ManagementQRCode
{
    public interface IGanarateQRCode
    {
        public byte[] GenerateQRCodeForBookingDetail(string CodeBooking);
    }
}
