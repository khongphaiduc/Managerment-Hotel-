
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace RabbitMQConsumer.Email
{
    public class EmailService : INotifications
    {
        public Task<bool> SendBookingFailureNotification(string message, string recipient)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendBookingSuccessNotification(string toEmail, string subject, string body)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("hotelluxurytrungduc@gmail.com", "ykbg blmo tqxy hrld");
                    smtp.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("hotelluxurytrungduc@gmail.com", "Hotel Management");
                        message.To.Add(toEmail);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await smtp.SendMailAsync(message);
                    }
                }

                Console.WriteLine("EmailService sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }

        //gửi mã qr kèm theo email
        public async Task<bool> SendBookingSuccessNotification(string toEmail, string subject, string body, byte[] qrCodeBytes)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("hotelluxurytrungduc@gmail.com", "ykbg blmo tqxy hrld"); // App password Gmail
                    smtp.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("hotelluxurytrungduc@gmail.com", "Khách sạn Luxury Trung Đức");
                        message.To.Add(toEmail);
                        message.Subject = subject;
                        message.IsBodyHtml = true;

                        // 1️⃣ Tạo LinkedResource từ byte[] QR
                        LinkedResource qrImage = new LinkedResource(new MemoryStream(qrCodeBytes), MediaTypeNames.Image.Jpeg);
                        qrImage.ContentId = "QrCodeImage";
                        qrImage.TransferEncoding = TransferEncoding.Base64;

                        // 2️⃣ Tạo nội dung HTML có ảnh nhúng
                        string htmlBody = $@"
                {body}
                <div style='text-align:center;margin-top:10px;'>
                    <img src='cid:QrCodeImage' alt='QR Code đặt phòng' width='180' height='180' />
                </div>

<p>Nếu Quý khách có bất kỳ yêu cầu đặc biệt hoặc cần hỗ trợ thêm, xin vui lòng liên hệ với chúng tôi qua:<br>
📞 Hotline: 033333333<br>
📧 EmailService: hotelluxurytrungduc@gmail.com</p>

<p>Một lần nữa, xin cảm ơn Quý khách đã lựa chọn <b>Khách sạn Luxury Trung Đức</b>.<br>
Chúng tôi hân hạnh được đón tiếp Quý khách!</p>

<p>Trân trọng,<br>
<b>Khách sạn Luxury Trung Đức</b></p>
";

                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                        htmlView.LinkedResources.Add(qrImage);

                        message.AlternateViews.Add(htmlView);

                        await smtp.SendMailAsync(message);
                    }
                }

                Console.WriteLine("✅ EmailService sent successfully with QR code!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error sending email: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> SendNotificationResetPassword(string toEmail, string subject, string password)
        {
            string htmlBody = $@"
<div style='max-width:600px;margin:20px auto;padding:20px;border:1px solid #e6e6e6;border-radius:8px;font-family:Arial,Helvetica,sans-serif;color:#333;background-color:#fff;'>

    <div style='text-align:center;'>
        <h2 style='margin:0 0 8px 0;'>Mật khẩu mới của bạn</h2>
        <p style='margin:0 0 18px 0;font-size:14px;color:#666;'>Xin chào <strong>{toEmail}</strong>, mật khẩu mới của bạn đã được tạo.</p>
    </div>

    <div style='text-align:center;margin:18px 0;padding:12px;background:#f0f8ff;border-radius:6px;font-size:16px;color:#0d6efd;font-weight:bold;'>
        {password}
    </div>

    <div style='margin-top:18px;font-size:13px;color:#444;line-height:1.45;'>
        <p style='margin:0 0 8px 0;'>Vui lòng sử dụng mật khẩu này để đăng nhập và nên thay đổi mật khẩu sau khi đăng nhập để bảo mật tài khoản.</p>

        <p style='margin:10px 0 0 0;'>Nếu bạn không yêu cầu thay đổi mật khẩu, vui lòng liên hệ với chúng tôi ngay lập tức.</p>

        <p style='margin:10px 0 0 0;'>Thông tin liên hệ:<br>
        📞 Hotline: <strong>033333333</strong><br>
        📧 EmailService: <strong>hotelluxurytrungduc@gmail.com</strong></p>
    </div>

    <div style='text-align:left;margin-top:18px;font-size:13px;color:#666;'>
        <p style='margin:0;'>Trân trọng,<br>
        <strong>Khách sạn Luxury Trung Đức</strong></p>
    </div>

</div>";


            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("hotelluxurytrungduc@gmail.com", "ykbg blmo tqxy hrld");
                    smtp.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("hotelluxurytrungduc@gmail.com", "Hotel Management");
                        message.To.Add(toEmail);
                        message.Subject = subject;
                        message.Body = htmlBody;
                        message.IsBodyHtml = true;

                        await smtp.SendMailAsync(message);
                    }
                }

                Console.WriteLine("EmailService sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }


    }
}
