using Microsoft.Identity.Client;

namespace Management_Hotel_2025.Modules.Notifications.NotificationsSevices
{
    public interface INotifications
    {
        Task<bool> SendBookingSuccessNotification(string toEmail, string subject, string body);

        Task<bool> SendBookingFailureNotification(string message, string recipient);

        Task<bool> SendBookingSuccessNotification(string toEmail, string subject, string body, byte[] qrCodeBytes);

        Task<bool> SendNotificationResetPassword(string toEmail, string subject, string body);
    }
}
