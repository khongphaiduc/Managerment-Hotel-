using Microsoft.AspNetCore.SignalR;
using Mydata.Models;

namespace Management_Hotel_2025.Modules.SignalRModels
{
    public class TaiXiuGames : Hub
    {
        private static int _connectedUsers = 0; // số  lượng người đang online

        private readonly IHttpContextAccessor _httpContextAccessor;

        string userName = "";
        public TaiXiuGames(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext?.User;
            userName = user?.FindFirst("FullName")?.Value;
        }

        public override async Task OnConnectedAsync()
        {
            _connectedUsers++;
           
            await Clients.Caller.SendAsync("JoinedGame", "Welcome!");              //Clients.Caller chỉ đại diện cho client vừa gọi Hub method.
            await Clients.All.SendAsync("UserCountUpdated", _connectedUsers);      // gửi số lượng người chơi mới nhất
            await Clients.All.SendAsync("AdminInformJoin", userName);
        
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedUsers--;
            if (_connectedUsers < 0) _connectedUsers = 0;


            await Clients.All.SendAsync("UserCountUpdated", _connectedUsers);
            await Clients.All.SendAsync("AdminInformOut", userName);
            await base.OnDisconnectedAsync(exception);
        }

        // Gửi tin nhắn này tới tất cả client đang kết nối
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
