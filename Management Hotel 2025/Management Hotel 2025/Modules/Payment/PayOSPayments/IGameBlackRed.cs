namespace Management_Hotel_2025.Modules.Payment.PayOSPayments
{
    public interface IGameBlackRed
    {
        Task<bool> AddCoinForUsers(int userId, long coinAmount);  // thêm coiin 

        Task<bool> RemoveCoinForUsers(int userId, long coinAmount);  // xóa coin


        Task<decimal> GetCoinsForUsers(int userId);              // lấy toàn bộ coin của user
    }
}
