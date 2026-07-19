using ApiStatistics = API_BookingHotel.Modules.Statistics.StatisticsServices;
using Microsoft.AspNetCore.Mvc;

namespace Management_Hotel_2025.Modules.Statistics.StatisticsControllers;

[Route("admin")]
public class MStatisticsController : Controller
{
    private readonly ApiStatistics.IStatisticsServices _statisticsService;

    public MStatisticsController(ApiStatistics.IStatisticsServices statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("statistics")]
// Loads statistics from the statistics service and displays the admin dashboard.
    public async Task<IActionResult> ViewStatistics()
    {
        var result = await _statisticsService.GetStatisticsAsync();
        return View(result);
    }
}
