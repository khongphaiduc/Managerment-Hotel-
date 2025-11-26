using Microsoft.AspNetCore.SignalR;
//
namespace Management_Hotel_2025.Modules.SignalRModels
{
    public class CoinHub : Hub  // lúc này thì CoinHub chính là 1 Hub (đứng giữa) giúp kết nối server và client
    {

        // các phương thức trong Hub, chỉ dùng khi client gọi đến server

        public async Task NotifyCoinUpdated(string userId, int newCoinAmount, string message)
        {
            // ví dụ client gọi phương thức này để thông báo số coin đã được cập nhật

            // xử lý logic xong thì sẽ gửi lại dữ liệu về cho client
            await Clients.User(userId).SendAsync("ReceiveCoinUpdate", newCoinAmount, message);   // tham số đầu tiên là tên phương thức trên client sẽ nhận dữ liệu từ server còn lại sẽ là dữ liệu gửi kèm
        }


        public async Task UpdateCoin(string userId, int totalcoin)
        {
            await Clients.User(userId).SendAsync("UpdateCoin", totalcoin);
        }

    }
}
