

using Mydata.Models;
using System.Text.RegularExpressions;

namespace Management_Hotel_2025.Serives.AuthenSerive
{
    public class ValidationAuthen
    {
        private readonly ManagermentHotelContext _Idbcontext;

        public ValidationAuthen(ManagermentHotelContext Icontext)
        {
            _Idbcontext = Icontext;


        }

        public ValidationAuthen()
        {
        }

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false; // Email is empty
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true; // Valid email
            }
            catch
            {
                return false; // Invalid email
            }
        }




        public bool ValidatePassword(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
            // Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one digit
        }


        public bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            return Regex.IsMatch(phoneNumber, @"^\d{10,}$");
        }


        public bool ExistPhoneNumber(string phoneNumber)
        {
            return _Idbcontext.Users.Any(u => u.PhoneNumber == phoneNumber);
        }


    }
}
