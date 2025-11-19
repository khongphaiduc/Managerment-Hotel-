using API_BookingHotel.Modules.Statistics.StatisticsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BookingHotel.Modules.Statistics.StatisticsControllers
{
    [Route("admin")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsServices _Ista;

        public StatisticsController(IStatisticsServices statistics)
        {
            _Ista = statistics;
        }

        [HttpGet("statist")]
        public async Task<IActionResult> GetStatis()
        {

            var resultStatis = await _Ista.GetStatisticsAsync();

            return Ok(resultStatis);
        }



    }
}
