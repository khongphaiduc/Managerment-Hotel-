

using Management_Hotel_2025.Modules.Rooms.RoomViewModel;

using Management_Hotel_2025.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mydata.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Management_Hotel_2025.Modules.Rooms.RoomService
{
    public class RoomSerices : IRoomService
    {
        private readonly ManagermentHotelContext _dbcontext;
 
        private readonly IHttpClientFactory _IhttpClient;
        private readonly IConfiguration _Iconfig;

        public RoomSerices(IConfiguration configuration, IHttpClientFactory httpClientFactory, ManagermentHotelContext managermentHotelContext)
        {
            _dbcontext = managermentHotelContext;        
            _IhttpClient = httpClientFactory;
            _Iconfig = configuration;
        }


        public HttpClient gethttpClient()
        {
            return _IhttpClient.CreateClient();
        }

        public bool AddServicesHotel(int IdService)
        {
            throw new NotImplementedException();
        }
        // Lấy danh sách phòng của người dùng theo userId
        public List<ViewListRoomsOfUser> GetListRoomOfUser(int userId)
        {
            var listItem = _dbcontext.Bookings
       .Include(s => s.BookingDetails)
           .ThenInclude(d => d.Room)
               .ThenInclude(r => r.RoomType)
       .Where(s => s.UserId == userId && s.Status == "Success")
       .Select(s => new ViewListRoomsOfUser()
       {
           IdRoom = s.BookingDetails.Select(b => b.Room.RoomId).FirstOrDefault(),
           NumberRoom = Convert.ToInt32(s.BookingDetails.Select(s => s.Room.RoomNumber).FirstOrDefault()),
           NameRoom = s.BookingDetails.Select(b => b.Room.RoomType.Name).FirstOrDefault(),
           StatusRoom = s.BookingDetails.Select(b => b.StatusCheckRoom).FirstOrDefault(),
           Floor = s.BookingDetails.Select(b => b.Room.Floor).FirstOrDefault(),
           PriceRoom = s.BookingDetails.Select(b => b.Room.RoomType.Price).FirstOrDefault(),
           DescriptionRoom = s.BookingDetails.Select(b => b.Room.Description).FirstOrDefault(),
           ImageRoom = s.BookingDetails.Select(b => b.Room.PathImage).FirstOrDefault(),
           DateCheckIn = s.BookingDetails.Select(b => b.CheckInDate).FirstOrDefault(),
           DateCheckout = s.BookingDetails.Select(b => b.CheckOutDate).FirstOrDefault(),

       }).ToList();


            if (listItem.IsNullOrEmpty())
            {
                return new List<ViewListRoomsOfUser>();
            }
            else
            {
                return listItem;
            }
        }

        // Xem chi tiết phòng theo IdRoom
        public async Task<ViewDetailRoom> ViewDetailOfRoom(int IdRoom)
        {
            using (var httpclient = gethttpClient())
            {
                string url = _Iconfig["ApiHotel:ViewDetailRoom"] + $"/{IdRoom}";

                var respon = await httpclient.GetAsync(url);

                if (respon.IsSuccessStatusCode)
                {

                    var data = await respon.Content.ReadAsStringAsync();

                    var room = JsonSerializer.Deserialize<ViewDetailRoom>(data,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new ViewDetailRoom();

                    return room;
                }
                else
                {
                    return new ViewDetailRoom();
                }


            }


        }
    }
}
