
using Management_Hotel_2025.Modules.AuthenSerive;
using Management_Hotel_2025.Modules.Notifications.NotificationsSevices;
using Management_Hotel_2025.Modules.RabbitMQHotel;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using System.Security.Cryptography;
using System.Text;

namespace Management_Hotel_2025.Serives.AuthenSerive
{
    public class RegisterAccount
    {
        private readonly ManagermentHotelContext _dbcontext;
        private readonly IEncoding _Iendcoding;
        private readonly INotifications _notification;
        private readonly ILogger<RegisterAccount> _logger;
        private readonly EmailProducer _rabbitMQ;
        private readonly IConfiguration _iconfig;

        public RegisterAccount(IConfiguration configuration, ManagermentHotelContext dbcontext, IEncoding iencoding, INotifications notifications, ILogger<RegisterAccount> logger, EmailProducer rabbitMQServices)
        {
            _dbcontext = dbcontext;
            _Iendcoding = iencoding;
            _notification = notifications;
            _logger = logger;
            _rabbitMQ = rabbitMQServices;
            _iconfig = configuration;
        }

        public bool Register(string username, string phone, string email, string password)
        {
            try
            {
                if (EmailExists(email))
                {
                    return false;
                }
                else
                {
                    string satl = _Iendcoding.GenerateSalt();

                    var user = new User
                    {
                        Username = username,
                        FullName = username,
                        PhoneNumber = phone,
                        Email = email,
                        PasswordHash = _Iendcoding.HashPassword(password, satl),
                        Salt = satl,
                        Role = "User",
                        CreatedAt = DateTime.Now
                    };
                    _dbcontext.Users.Add(user);
                    _dbcontext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool EmailExists(string email)
        {
            return _dbcontext.Users.Any(u => u.Email == email);
        }



        public async Task<bool> ResetPassword(string email)
        {

            var user = await _dbcontext.Users.FirstOrDefaultAsync(s => s.Email == email);

            if (user == null)
            {
                Console.WriteLine("User NUll");
                return false;
            }
            else
            {
                _logger.LogInformation($"User là {user.Email}");
                var listchar = "zxcvbnmasdfghjklqwertyuyiop1234567890";

                char[] chars = new char[listchar.Length];
                Random random = new Random();
                for (int i = 0; i < listchar.Length; i++)
                {
                    int index = random.Next(0, listchar.Length);
                    chars[i] = listchar[index];
                }

                string password = new string(chars);
                string satl = _Iendcoding.GenerateSalt();

                string passwordHash = _Iendcoding.HashPassword(password, satl);

                user.PasswordHash = passwordHash;
                user.Salt = satl;

                await _rabbitMQ.SendMessages(new RabbitMQMessages
                {
                    To = user.Email,
                    Subject = "Thay đổi mật khẩu",
                    Body = password,
                    Type = _iconfig["Status:ResetPassword"]!
                });

            }
            return await _dbcontext.SaveChangesAsync() > 0;

        }


    }
}
