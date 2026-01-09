
using Management_Hotel_2025.Modules.AdminMPassengers.MPassengersServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Mydata.Models;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers
{

    [Route("admin")]
    public class AdminManagementPassengersController : Controller
    {
        private readonly IConfiguration _Iconfi;
        public string apiPassengers = "";
        private readonly IHttpClientFactory _httpClient;
        private readonly IAdminMPassengers _IadminMPassgers;

        public AdminManagementPassengersController(IConfiguration configuration, IAdminMPassengers admin, IHttpClientFactory httpClientFactory)
        {
            _IadminMPassgers = admin;
            _Iconfi = configuration;
            apiPassengers = _Iconfi["ApiHotel:PassengerInfo"];
            _httpClient = httpClientFactory;
        }


        public string token
        {
            get
            {
                return HttpContext.Session.GetString("token")!;
            }
        }

        public HttpClient GetHttpClient()
        {

            var httpclient = _httpClient.CreateClient();
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            return httpclient;
        }

        // xem danh sách khách hangh
        [HttpGet("passengers")]
        public async Task<IActionResult> ViewListPassenger()
        {

            var listPassengers = await _IadminMPassgers.GetListViewPassengers();

            return View(listPassengers);
        }

        // xem chi tiết hành khách
        [HttpGet("passengers/{codePassenger}")]
        public async Task<IActionResult> GetPassengersInfo(string codePassenger)
        {

            string url = apiPassengers + "/" + codePassenger;

            try
            {


                using var httpclient = GetHttpClient();

                var response = await httpclient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    var passengerList = Newtonsoft.Json.JsonConvert.DeserializeObject<PassengerDetail>(data);

                    return View(passengerList);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error retrieving passengers");
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
