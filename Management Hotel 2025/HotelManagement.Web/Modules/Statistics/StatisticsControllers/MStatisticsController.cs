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
    // Handles the ViewStatistics action.
    public async Task<IActionResult> ViewStatistics()
    {
        var result = await _statisticsService.GetStatisticsAsync();
        return View(result);
    }
}
