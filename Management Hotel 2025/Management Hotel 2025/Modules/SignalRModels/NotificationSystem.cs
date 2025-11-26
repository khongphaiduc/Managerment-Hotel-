using Microsoft.AspNetCore.SignalR;

namespace Management_Hotel_2025.Modules.SignalRModels
{
    //  dùng để thông báo hệ thống realtime
    public class NotificationSystem : Hub
    {

        //public static Dictionary<string, string> MapIDwithConnnectID = new Dictionary<string, string>();

        //public Task RegisterUserID(string codeIdentityUser)
        //{
        //    var connectionId = Context.ConnectionId;
        //    if (!MapIDwithConnnectID.ContainsKey(codeIdentityUser))
        //    {
        //        MapIDwithConnnectID.Add(codeIdentityUser, connectionId); // chưa có thì thêm mới 
        //    }
        //    else
        //    {
        //        MapIDwithConnnectID[codeIdentityUser] = connectionId;// có rồi thì ghi đè
        //    }
        //    return Task.CompletedTask;
        //}

    }
}
