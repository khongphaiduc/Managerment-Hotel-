
using API_BookingHotel.Modules.Rooms.DTOs;

using Management_Hotel_2025.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Mydata.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Rooms.RoomsController
{
    [Route("room")]
    public class ManagementRoomController : Controller
    {
        private readonly ManagermentHotelContext _dbContext;

        private readonly HttpClient _httpClient;
        private readonly ILogger<ManagementRoomController> _logger;
        private readonly IHttpClientFactory _IhttpClientF;
        private readonly IConfiguration _iconfig;

        public ManagementRoomController(IConfiguration configuration, ManagermentHotelContext dbcontext, HttpClient httpClient, ILogger<ManagementRoomController> logger, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbcontext;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
            _IhttpClientF = httpClientFactory;
            _iconfig = configuration;
        }

        public HttpClient gethttpClient()
        {

            var httpclient = _IhttpClientF.CreateClient();

            return httpclient;

        }


        // hiện thi danh sách phòng(advance) và phân trang
        [AllowAnonymous]
        [Route("list/{PageCurrent}/{NumerItemOfPage}")]
        public async Task<IActionResult> ViewListRoomVer2(RoomFilterRequest roomrequest)
        {
            ViewBag.Floor = roomrequest.Floor;
            ViewBag.PageCurrent = roomrequest.PageCurrent;
            ViewBag.NumerItemOfPage = roomrequest.NumerItemOfPage;
            ViewBag.PriceMin = roomrequest.PriceMin;
            ViewBag.PriceMax = roomrequest.PriceMax;
            ViewBag.Person = roomrequest.Person;

            var Today = DateTime.Now;

            if (string.IsNullOrEmpty(roomrequest.StartDate) || string.IsNullOrEmpty(roomrequest.EndDate))
            {
                roomrequest.StartDate = Today.ToString("yyyy-MM-dd");
                roomrequest.EndDate = Today.AddDays(7).ToString("yyyy-MM-dd");
            }

            ViewBag.StartDate = roomrequest.StartDate;
            ViewBag.EndDate = roomrequest.EndDate;

            // lưu ngày checkin và out vào session
            HttpContext.Session.SetString("StartDate", roomrequest.StartDate);
            HttpContext.Session.SetString("EndDate", roomrequest.EndDate);


            using (var httpclient = gethttpClient())
            {

                string urlapi = _iconfig["ApiHotel:RoomHotel"] + $"?PageCurrent={roomrequest.PageCurrent}&NumerItemOfPage={roomrequest.NumerItemOfPage}&Floor={roomrequest.Floor}&PriceMin={roomrequest.PriceMin}&PriceMax={roomrequest.PriceMax}&Person={roomrequest.Person}&StartDate={roomrequest.StartDate}&EndDate={roomrequest.EndDate}";

                var respone = await httpclient.GetAsync(urlapi);

                if (respone.IsSuccessStatusCode)
                {

                    var dataRespone = await respone.Content.ReadAsStringAsync();

                    var model = JsonSerializer.Deserialize<PaginatedResult<ViewRoomModel>>(dataRespone,
                          new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(model);
                }
                else
                {
                    return NotFound("No rooms found.");
                }

            }

        }

        [AllowAnonymous]
        [Route("detail")]
        public async Task<IActionResult> ViewDetailRoomVer2([FromQuery] int IdRoom)
        {

            using (var httpclient = gethttpClient())
            {
                string url = _iconfig["ApiHotel:ViewDetailRoom"] + $"/{IdRoom}";

                var respon = await httpclient.GetAsync(url);

                if (respon.IsSuccessStatusCode)
                {

                    var data = await respon.Content.ReadAsStringAsync();

                    var room = JsonSerializer.Deserialize<ViewDetailRoom>(data,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new ViewDetailRoom();

                    return View(room);
                }
                else
                {
                    return View(new ViewDetailRoom()
                    {

                    });
                }

            }

        }


        [Route("date")]
        public IActionResult ChosseDate()
        {
            return View();
        }


    }
}
