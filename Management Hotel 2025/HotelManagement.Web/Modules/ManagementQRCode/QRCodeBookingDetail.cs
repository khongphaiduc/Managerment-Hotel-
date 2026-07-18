using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

namespace Management_Hotel_2025.Modules.ManagementQRCode
{
    public class QRCodeBookingDetail : IGanarateQRCode
    {

        public byte[] GenerateQRCodeForBookingDetail(string CodeBooking)
        {
            string bookingUrl = $"https://localhost:7045/StaffManagementRoom/ViewDetailBooking?Code={CodeBooking}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(bookingUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                qrBitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
