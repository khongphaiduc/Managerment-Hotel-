
using Management_Hotel_2025.Modules.AuthenSerive;
using Microsoft.AspNetCore.Identity.Data;
using Mydata.Models;


namespace Management_Hotel_2025.Serives.AuthenSerive
{
    public class Login
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IEncoding _encode;

        public Login()
        {
        }

        public Login(ManagermentHotelContext dbccontext,IEncoding myendcode)
        {
            _dbcontext = dbccontext;
            _encode = myendcode;
        }


        public bool MyLogin(string email, string password)
        {
            var user = _dbcontext.Users
                .Where(s => s.Email == email)
                .Select(s => new { s.Salt, s.PasswordHash })
                .FirstOrDefault();

            if (user == null) return false;

            var hashedPassword = _encode.HashPassword(password, user.Salt);

            return hashedPassword.Equals(user.PasswordHash);
        }



    }
}
