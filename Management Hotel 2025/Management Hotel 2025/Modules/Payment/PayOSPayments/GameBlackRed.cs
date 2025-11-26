
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Mydata.Models;

namespace Management_Hotel_2025.Modules.Payment.PayOSPayments
{
    public class GameBlackRed : IGameBlackRed
    {
        private readonly ManagermentHotelContext _dbcontext;

        public GameBlackRed(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // cộng tiền
        public async Task<bool> AddCoinForUsers(int userId, long coinAmount)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                user.Coin += coinAmount;

                _dbcontext.Notifications.Add(new Notification()
                {
                    UserId = userId,
                    Message = $"Bạn đã nạp thành công {coinAmount} xu vào tài khoản.",
                    CreatedAt = DateTime.Now
                });

            }

            return await _dbcontext.SaveChangesAsync() > 0;
        }


        // trừ tiền
        public async Task<bool> RemoveCoinForUsers(int userId, long coinAmount)
        {
            var item = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (item != null)
            {
                item.Coin -= coinAmount;

                _dbcontext.Notifications.Add(new Notification()
                {
                    UserId = userId,
                    Message = $"Bạn đã bị trừ {coinAmount} xu vào tài khoản.",
                    CreatedAt = DateTime.Now
                });

            }
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        public async Task<decimal> GetCoinsForUsers(int userId)
        {

            try
            {

                var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userId);


                if (user != null)
                {
                    return user.Coin;

                }

            }
            catch (Exception)
            {

                throw;
            }

            throw new NotImplementedException();
        }
    }
}
