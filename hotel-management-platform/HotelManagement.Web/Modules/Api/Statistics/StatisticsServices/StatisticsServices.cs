using API_BookingHotel.Modules.Statistics.StatisticsModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Mydata.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_BookingHotel.Modules.Statistics.StatisticsServices
{
    public class StatisticsServices : IStatisticsServices
    {
        private readonly ManagermentHotelContext _dbcontext;

        public StatisticsServices(ManagermentHotelContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<StatisticsViewModel> GetStatisticsAsync()
        {

            var statistics = new StatisticsViewModel
            {
                allBookingsByYear = new DataYears
                {
                    Data = new Dictionary<int, List<int>>()
                }
                ,
                allRevenuesByYear = new DataYears
                {
                    Data = new Dictionary<int, List<int>>()
                }
                ,
                roomRevenuesByYear = new DataYears
                {
                    Data = new Dictionary<int, List<int>>()
                }
                ,
                serviceRevenuesByYear = new DataYears
                {
                    Data = new Dictionary<int, List<int>>()
                }
                ,
                genderByYear = new DataYears

                { Data = new Dictionary<int, List<int>>() }
            };


            for (int year = DateTime.Now.Year; year > DateTime.Now.Year - 3; year--)
            {
                var rawData = await _dbcontext.Bookings
                    .Where(b => b.BookingDate.Year == year)
                    .GroupBy(b => b.BookingDate.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var monthlyData = Enumerable.Range(1, 12)
                    .Select(month => rawData.FirstOrDefault(d => d.Month == month)?.Count ?? 0)
                    .ToList();

                statistics.allBookingsByYear.Data[year] = monthlyData;
            }

            return statistics;
        }
    }
}
