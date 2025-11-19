using Management_Hotel_2025.Modules.AdminMPassengers.AdminMPassengerControllers;
using Management_Hotel_2025.Modules.Statistics.StatisticsModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Management_Hotel_2025.Modules.Statistics.StatisticsControllers
{
    [Route("admin")]
    public class MStatisticsController : Controller
    {
        private readonly IConfiguration _iconfig;
        private readonly string? _apiurl;

        public MStatisticsController(IConfiguration configuration)
        {
            _iconfig = configuration;
            _apiurl = configuration["ApiHotel:Statistic"];
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> ViewStatistics()
        {

            try
            {
                using (var httpclient = new HttpClient())
                {

                    var respone = await httpclient.GetAsync(_apiurl);


                    if (respone.IsSuccessStatusCode)
                    {
                        var data = await respone.Content.ReadAsStringAsync();

                        var resultStattis = Newtonsoft.Json.JsonConvert.DeserializeObject<StatisticsViewModel>(data);

                        return View(resultStattis);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }


            return View();

        }
    }
}
